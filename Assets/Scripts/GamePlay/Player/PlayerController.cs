// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_08
// File: PlayerController.cs
// Description: 玩家角色主要控制逻辑
// -------------------------------------------------

using System;
using System.Collections.Generic;
using Common.FSM;
using GamePlay.Player.Particle;
using GamePlay.Player.PlayerInput;
using GamePlay.Player.PlayerState;
using PurpleFlowerCore;
using Sirenix.OdinInspector;
using UnityEngine;

// 考虑到玩家状态较多，各种子状态需要考虑有无连接或其他情况，舌头本身也有多种状态
// 我们使用并行的两个状态机，一个用于玩家，一个用于舌头，两状态机之间的影响和数据传递通过独立拉出的玩家数据类实现
namespace GamePlay.Player
{
    public delegate void PlayerFlap();
    
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerHead head;
        public PlayerHead Head => head;
        [SerializeField] private Transform entity;
        public Transform Entity => entity;

        private PlayerProperty property;
        public PlayerProperty Property => property;

        [SerializeField] private PlayerConfig config;
        public PlayerConfig Config
        {
            get => config;
            set => config  = value;
        }
        
        private HStateMachine _stateMachine;
        public HStateMachine StateMachine => _stateMachine;
        
        [SerializeField] private SpriteRenderer spriteRenderer;
        public SpriteRenderer SpriteRenderer => spriteRenderer;

        [SerializeField] private Animator animator;
        public Animator Animator => animator;
        
        [SerializeField] private Rigidbody2D rb;
        public Rigidbody2D Rb => rb;
        
        private PlayerInputBase _input;
        public PlayerInputBase Input => _input;
        [SerializeField] private SpriteRenderer entityBackground;
        public SpriteRenderer EntityBackground => entityBackground;
        
        [SerializeField] private PlayerParticleProxy playerParticle;
        public PlayerParticleProxy PlayerParticle => playerParticle;
        
        public List<RuntimeAnimatorController> controllers = new();
         public string CurrentStateName => _stateMachine.CurrentState.Name;
        
        [SerializeField] private Transform groundCheckPoint; // 地面检测点
        [SerializeField] private Transform wallCheckPoint1; // 墙壁检测点
        [SerializeField] private Transform wallCheckPoint2; // 墙壁检测点
        [SerializeField] private Transform cameraPoint;
        public Transform CameraPoint => cameraPoint;

        public PlayerFlap PlayerFlap;
        
        private PlayerGravityScaleProxy _playerGravityScaleProxy;

        public event Action<Collision2D> OnCollisionEnter;
        public event Action<Collision2D> OnCollisionExit;

        [Title("Others")]
        [SerializeField] private Transform keyFollowPoint;
        public Transform KeyFollowPoint => keyFollowPoint;

        private void Update()
        {
            CheckState();
            StateMachine.UpdateCallback(Time.deltaTime);
            property.Update(Time.deltaTime);
            _playerGravityScaleProxy.Update();
        }

        private void LateUpdate()
        {
            PlayerFlap?.Invoke();
            StateMachine.LateUpdateCallback(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            StateMachine.FixedUpdateCallback();
            property.FixedUpdate(Time.deltaTime);
            _input.FixedUpdate();
        }

        /// <summary>
        /// 原则是，我们确保关于Player的逻辑模块都在PlayerController的Init之后初始化
        /// </summary>
        public void Init()
        {
            property = new(this);
            property.OnColorChanged += ChangeColor;
            _input = new PlayerInput_Legacy(this);
            _playerGravityScaleProxy = new PlayerGravityScaleProxy(this);
            
            // 定义整个状态机
            // 状态
            OnGroundState onGroundState = new OnGroundState(this, EPlayerState.OnGround);
            AirState airState = new AirState(this, EPlayerState.Air);
            HangState hangState = new HangState(this, EPlayerState.Hang);
            OnWallState onWallState = new OnWallState(this, EPlayerState.OnWall);
            SmashState smashState = new SmashState(this, EPlayerState.Smash);
            OnPillarState onPillarState = new OnPillarState(this, EPlayerState.OnPillar);
            SwimState swimState = new SwimState(this, EPlayerState.Swim);
            // todo: 自定义状态
            InCannonState inCannonState = new InCannonState(this, EPlayerState.InCannon);
            ShuttleState shuttleState = new ShuttleState(this, EPlayerState.Shuttle);
            
            _stateMachine = new HStateMachine(onGroundState);

            // 转移 todo: 定义转移写法的修改
            HTransition jump = new HTransition("Jump", onGroundState, airState);
            jump.OnCheck += () => !property.IsGrounded;
            onGroundState.AddTransition("Jump", airState, () => !property.IsGrounded);

            HTransition land = new HTransition("Land", airState, onGroundState);
            land.OnCheck += () => property.IsGrounded;
            airState.AddTransition(land);

            HTransition connect = new HTransition("Connect", airState, hangState);
            connect.OnCheck += () => property.IsConnecting;
            airState.AddTransition(connect);

            HTransition hangJump = new HTransition("HangJump", hangState, airState);
            hangJump.OnCheck += () => !property.IsConnecting;
            hangState.AddTransition(hangJump);

            HTransition onWall = new HTransition("OnWall", airState, onWallState);
            onWall.OnCheck += () => property.IsNearWall && !property.OnWallFlag && 
                                    (property.IsRightWall && Input.XInputExtent > 0.1f
                                     || !property.IsRightWall && Input.XInputExtent < -0.1f);
            airState.AddTransition(onWall);

            HTransition wallJump = new HTransition("WallJump", onWallState, airState);
            wallJump.OnCheck += () => !property.IsNearWall;
            onWallState.AddTransition(wallJump);

            HTransition wallLeave = new HTransition("WallLeave", onWallState, airState);
            wallLeave.OnCheck += () => property.IsNearWall && 
                                       (property.IsRightWall && Input.NoRightTime > config.wallExitCoefficient 
                                        || !property.IsRightWall && Input.NoLeftTime > config.wallExitCoefficient);
            onWallState.AddTransition(wallLeave);

            HTransition wallToGround = new HTransition("WallToGround", onWallState, onGroundState);
            wallToGround.OnCheck += () => property.IsGrounded;
            onWallState.AddTransition(wallToGround);

            HTransition smash = new HTransition("Smash", airState, smashState);
            smash.OnCheck += () => Input.DownInputDown && property.SmashFlag;
            airState.AddTransition(smash);
            
            HTransition smashToAir = new HTransition("SmashToAir", smashState, airState);
            smashToAir.OnCheck += () => property.HasSmashLanded;
            smashState.AddTransition(smashToAir);

            HTransition climb = new HTransition("Climb", onGroundState, onPillarState);
            climb.OnCheck += () => property.CanOnPillar && Input.UpInput && property.ClimbFlag;
            onGroundState.AddTransition(climb);

            HTransition climbLand = new HTransition("ClimbLand", onPillarState, onGroundState);
            climbLand.OnCheck += () => Input.DownInput && property.IsGrounded;
            onPillarState.AddTransition(climbLand);

            HTransition climbJump = new HTransition("ClimbJump", onPillarState, airState);
            climbJump.OnCheck += () => Input.JumpInputDown;
            onPillarState.AddTransition(climbJump);

            HTransition climbToHang = new HTransition("ClimbToHang", onPillarState, hangState);
            climbToHang.OnCheck += () => property.IsConnecting;
            onPillarState.AddTransition(climbToHang);

            HTransition airClimb = new HTransition("AirClimb", airState, onPillarState);
            airClimb.OnCheck += () => property.CanOnPillar && Input.UpInput && property.ClimbFlag;
            airState.AddTransition(airClimb);

            HTransition airToBackground = new HTransition("AirToBackground", airState, swimState);
            airToBackground.OnCheck += () => property.CanOnSwimColorBlock && Input.InteractInput;
            airState.AddTransition(airToBackground);

            HTransition backgroundToAir = new HTransition("BackgroundToAir", swimState, airState);
            backgroundToAir.OnCheck += () => !property.CanOnSwimColorBlock;
            swimState.AddTransition(backgroundToAir);

            HTransition groundToBackground = new HTransition("GroundToBackground", swimState, swimState);
            groundToBackground.OnCheck += () => property.CanOnSwimColorBlock && Input.InteractInput;
            onGroundState.AddTransition(groundToBackground);

            // todo: 自定义状态
            HTransition inCannonToAir = new HTransition("InCannonToAir", inCannonState, airState);
            inCannonToAir.OnCheck += () => !property.IsInCannon;
            inCannonState.AddTransition(inCannonToAir);

            HTransition anyToInCannon = new HTransition("AnyToInCannon",null, inCannonState);
            anyToInCannon.OnCheck += () => property.IsInCannon;
            _stateMachine.AddAnyState(anyToInCannon);
            
            HTransition shuttleToAir = new HTransition("ShuttleToAir", shuttleState, airState);
            shuttleToAir.OnCheck += () => !property.IsShuttle;
            shuttleState.AddTransition(shuttleToAir);

            HTransition airToShuttle = new HTransition("AirToShuttle",airState, shuttleState);
            airToShuttle.OnCheck += () => property.IsShuttle;
            airState.AddTransition(airToShuttle);
            
            HTransition smashToShuttle = new HTransition("SmashToShuttle", smashState, shuttleState);
            smashToShuttle.OnCheck += () => property.IsShuttle;
            smashState.AddTransition(smashToShuttle);

            // 初始化状态机
            _stateMachine.AddState(airState);
            _stateMachine.AddState(hangState);
            _stateMachine.AddState(onWallState);
            _stateMachine.AddState(smashState);
            _stateMachine.AddState(onPillarState);
            _stateMachine.AddState(inCannonState);
            _stateMachine.AddState(shuttleState);
            
#if UNITY_EDITOR
        DebugSystem.AddCommand("Player/Color/None", () => { property.CurrentColor = EPlayerColor.None;});
        DebugSystem.AddCommand("Player/Color/Green", () => { property.CurrentColor = EPlayerColor.Green;});
        DebugSystem.AddCommand("Player/Color/Red", () => { property.CurrentColor = EPlayerColor.Red;});
        DebugSystem.AddCommand("Player/Color/Blue", () => { property.CurrentColor = EPlayerColor.Blue;});
#endif
            ChangeColor(property.CurrentColor, property.CurrentColor);
            ResetTransform();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            // if(other.gameObject.layer == LayerMask.NameToLayer("Ground"))
            // {
            //     OnCollisionEnter?.Invoke();
            // }
            OnCollisionEnter?.Invoke(other);
        }
        
        private void OnCollisionExit2D(Collision2D other)
        {
            // if(other.gameObject.layer == LayerMask.NameToLayer("Ground"))
            // {
            //     OnCollisionExit?.Invoke();
            // }
            OnCollisionExit?.Invoke(other);
        }

        /// <summary>
        /// 检查状态的替换由玩家本体负责，而不放入状态中
        /// </summary>
        private void CheckState()
        {
            property.IsGrounded = Physics2D.OverlapBox(groundCheckPoint.position, new Vector2(Config.groundCheckWidth,
                Config.groundCheckHeight), 0, Config.groundLayer);
            
            property.CurrentGroundCollider = Physics2D.OverlapBox(groundCheckPoint.position, new Vector2(Config.groundCheckWidth,
                Config.groundCheckHeight), 0, ~LayerMask.GetMask("Player") );
            
            var rightOverlap = Physics2D.OverlapCircle(wallCheckPoint2.position, Config.wallCheckRadius, Config.groundLayer);
            bool rightWall = rightOverlap != null && rightOverlap.CompareTag("Wall");
            property.IsRightWall = rightWall;
            var leftOverlap = Physics2D.OverlapCircle(wallCheckPoint1.position, Config.wallCheckRadius, Config.groundLayer);
            bool leftWall = leftOverlap != null && leftOverlap.CompareTag("Wall");
            property.IsNearWall = rightWall || leftWall;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(groundCheckPoint.position, new Vector3(Config.groundCheckWidth, Config.groundCheckHeight, 0));
            Gizmos.DrawWireSphere(wallCheckPoint1.position, Config.wallCheckRadius);
            Gizmos.DrawWireSphere(wallCheckPoint2.position, Config.wallCheckRadius);
        }

        private void ChangeColor(EPlayerColor from, EPlayerColor to)
        {
            int index = (int)to;
            if(index >= 0 && index < controllers.Count)
            {
                string ani = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
                PFCLog.Debug("PlayerController", $"ChangeColor: {from} -> {to}, ani: {ani}");
                float aniTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                animator.runtimeAnimatorController = controllers[(int)to];
                animator.enabled = true;
                animator.Play(ani, 0, aniTime);
                animator.Update(0.001f);
                spriteRenderer.color = Color.white;
            }
            else
            {
                switch (to)
                {
                    case EPlayerColor.None:
                        spriteRenderer.color = Color.white;
                        break;
                    case EPlayerColor.Green:
                        spriteRenderer.color = Color.green;
                        break;
                    case EPlayerColor.Red:
                        spriteRenderer.color = Color.red;
                        break;
                    case EPlayerColor.Blue:
                        spriteRenderer.color = Color.blue;
                        break;
                }
            }
            head.ChangeColor(from, to);
        }
        
        public void ResetTransform()
        {
            Entity.rotation = Quaternion.identity;
            Entity.localScale = new Vector3(Property.FacingDirection, 1, 1);
        }
        
        public void AddGravityEffect(string id, float scale, float timer = 10000f, int priority = 0)
        {
            _playerGravityScaleProxy.AddGravityEffect(id, scale, timer, priority);
        }
        
        public void RemoveGravityEffect(string id)
        {
            _playerGravityScaleProxy.RemoveGravityEffect(id);
        }
        
        public void RemoveAllGravityEffect()
        {
            _playerGravityScaleProxy.RemoveAllGravityEffect();
        }
        
        public bool CheckState(EPlayerState state)
        {
            return _stateMachine.CurrentState.Name == state.ToString();
        }
    }

    public enum EPlayerColor
    {
        None = 0,
        Green = 1,
        Red = 2,
        Blue = 3
    }
    
    public enum EPlayerState
    {
        OnGround = 0,
        Air = 1,
        Hang = 2,
        OnWall = 3,
        Smash = 4,
        OnPillar = 5,
        InCannon = 6,
        Swim = 7,
        Shuttle = 8
    }
}
