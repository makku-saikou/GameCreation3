// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_08
// File: PlayerController.cs
// Description: 玩家角色主要控制逻辑
// -------------------------------------------------

using Common.FSM;
using GamePlay.Player.PlayerInput;
using GamePlay.Player.PlayerState;
using PurpleFlowerCore;
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
        
        public PlayerFlap PlayerFlap;
        
        private PlayerInputBase _input;
        public PlayerInputBase Input => _input;
        public string CurrentStateName => _stateMachine.CurrentState.Name;
        [SerializeField] private Transform groundCheckPoint; // 地面检测点
        [SerializeField] private Transform wallCheckPoint1; // 墙壁检测点
        [SerializeField] private Transform wallCheckPoint2; // 墙壁检测点
        [SerializeField] private Transform cameraPoint;
        public Transform CameraPoint => cameraPoint;

        private void Awake()
        {
            Init();
        }

        private void Update()
        {
            PlayerFlap?.Invoke();
            CheckState();
            StateMachine.UpdateCallback(Time.deltaTime);
        }

        private void LateUpdate()
        {
            PlayerFlap?.Invoke();
        }

        private void FixedUpdate()
        {
            StateMachine.FixedUpdateCallback();
            RecoverMaxSpeed();
        }

        /// <summary>
        /// 原则是，我们确保关于Player的逻辑模块都在PlayerController的Init之后初始化
        /// </summary>
        private void Init()
        {
            //todo:
            property = new(this);
            property.OnColorChanged += ChangeColor;
            _input = new PlayerInput_Legacy();
            
            // 定义整个状态机
            // 状态
            OnGroundState onGroundState = new OnGroundState(this, "OnGround");
            AirState airState = new AirState(this, "Air");
            HangState hangState = new HangState(this, "Hang");
            OnWallState onWallState = new OnWallState(this, "OnWall");
            SmashState smashState = new SmashState(this, "Smash");
            OnPillarState onPillarState = new OnPillarState(this, "OnPillar");
            OnBackgroundState onBackgroundState = new OnBackgroundState(this, "OnBackground");
            InCannonState inCannonState = new InCannonState(this, "InCannon");
            
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
            // onWall.OnCheck += () => property.IsWallSliding && (property.IsRightWall && Input.MovementInput >= 0.5f ||
            //                                                    !property.IsRightWall && Input.MovementInput <= -0.5f);
            onWall.OnCheck += () => property.IsWallSliding && !property.OnWallFlag;
            airState.AddTransition(onWall);

            HTransition wallJump = new HTransition("WallJump", onWallState, airState);
            wallJump.OnCheck += () => !property.IsWallSliding;
            onWallState.AddTransition(wallJump);

            HTransition wallLeave = new HTransition("WallLeave", onWallState, airState);
            wallLeave.OnCheck += () => property.IsWallSliding && (property.IsRightWall && Input.MovementInput < -0.5f ||
                                                                 !property.IsRightWall && Input.MovementInput > 0.5f);
            onWallState.AddTransition(wallLeave);

            HTransition wallToGround = new HTransition("WallToGround", onWallState, onGroundState);
            wallToGround.OnCheck += () => property.IsGrounded;
            onWallState.AddTransition(wallToGround);

            HTransition smash = new HTransition("Smash", airState, smashState);
            smash.OnCheck += () => Input.DownInput;
            airState.AddTransition(smash);

            HTransition smashLand = new HTransition("SmashLand", smashState, onGroundState);
            smashLand.OnCheck += () => property.IsGrounded;
            smashState.AddTransition(smashLand);

            HTransition climb = new HTransition("Climb", onGroundState, onPillarState);
            climb.OnCheck += () => property.CanOnPillar && Input.UpInput;
            onGroundState.AddTransition(climb);

            HTransition climbLand = new HTransition("ClimbLand", onPillarState, onGroundState);
            climbLand.OnCheck += () => property.CanOnPillar && Input.DownInput && property.IsGrounded;
            onPillarState.AddTransition(climbLand);

            HTransition climbJump = new HTransition("ClimbJump", onPillarState, airState);
            climbJump.OnCheck += () => property.CanOnPillar && Input.JumpInputDown;
            onPillarState.AddTransition(climbJump);

            HTransition climbToHang = new HTransition("ClimbToHang", onPillarState, hangState);
            climbToHang.OnCheck += () => property.CanOnPillar && property.IsConnecting;
            onPillarState.AddTransition(climbToHang);

            HTransition airClimb = new HTransition("AirClimb", airState, onPillarState);
            airClimb.OnCheck += () => property.CanOnPillar && Input.UpInput;
            airState.AddTransition(airClimb);

            HTransition airToBackground = new HTransition("AirToBackground", airState, onBackgroundState);
            airToBackground.OnCheck += () => property.CanOnColorBlock && Input.UpInput;
            airState.AddTransition(airToBackground);

            HTransition backgroundToAir = new HTransition("BackgroundToAir", onBackgroundState, airState);
            backgroundToAir.OnCheck += () => !property.CanOnColorBlock;
            onBackgroundState.AddTransition(backgroundToAir);

            HTransition groundToBackground = new HTransition("GroundToBackground", onBackgroundState, onBackgroundState);
            groundToBackground.OnCheck += () => property.CanOnColorBlock;
            onGroundState.AddTransition(groundToBackground);


            HTransition inCannonToAir = new HTransition("InCannonToAir", inCannonState, airState);
            inCannonToAir.OnCheck += () => !property.IsInCannon;
            inCannonState.AddTransition(inCannonToAir);

            HTransition anyToInCannon = new HTransition("AnyToInCannon",null, inCannonState);
            anyToInCannon.OnCheck += () => property.IsInCannon;
            _stateMachine.AddAnyState(anyToInCannon);

            // 初始化状态机
            _stateMachine.AddState(airState);
            _stateMachine.AddState(hangState);
            _stateMachine.AddState(onWallState);
            _stateMachine.AddState(smashState);
            _stateMachine.AddState(onPillarState);
            _stateMachine.AddState(inCannonState);

#if UNITY_EDITOR
        DebugSystem.AddCommand("Player/Color/Orange", () => { property.CurrentColor = EPlayerColor.Orange;});
        DebugSystem.AddCommand("Player/Color/Green", () => { property.CurrentColor = EPlayerColor.Green;});
        DebugSystem.AddCommand("Player/Color/Red", () => { property.CurrentColor = EPlayerColor.Red;});
        DebugSystem.AddCommand("Player/Color/Blue", () => { property.CurrentColor = EPlayerColor.Blue;});
#endif
        }

        /// <summary>
        /// 检查状态的替换由玩家本体负责，而不放入状态中
        /// </summary>
        private void CheckState()
        {
            property.IsGrounded =
                Physics2D.OverlapBox(groundCheckPoint.position, new Vector2(Config.groundCheckWidth,
                    Config.groundCheckHeight), 0, Config.groundLayer);
            bool rightWall =
                Physics2D.OverlapCircle(wallCheckPoint2.position, Config.wallCheckRadius, Config.groundLayer);
            property.IsRightWall = rightWall;
            property.IsWallSliding = Physics2D.OverlapCircle(wallCheckPoint1.position, Config.wallCheckRadius,
                Config.groundLayer) || rightWall;
        }

        private void RecoverMaxSpeed()
        {
            property.XMaxSpeed = Mathf.Lerp(property.XMaxSpeed, Config.commonXMaxSpeed,
                Config.xMaxSpeedRecoverScale);
            property.YMaxSpeed = Mathf.Lerp(property.YMaxSpeed, Config.commonYMaxSpeed,
                Config.yMaxSpeedRecoverScale);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(groundCheckPoint.position, new Vector3(Config.groundCheckWidth, Config.groundCheckHeight, 0));
            Gizmos.DrawWireSphere(wallCheckPoint1.position, Config.wallCheckRadius);
            Gizmos.DrawWireSphere(wallCheckPoint2.position, Config.wallCheckRadius);
        }

        private void ChangeColor(EPlayerColor from, EPlayerColor to)
        {
            if (spriteRenderer == null) return;
            switch (to)
            {
                case EPlayerColor.Orange:
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
    }

    public enum EPlayerColor
    {
        Orange = 0,
        Green = 1,
        Red = 2,
        Blue = 3
    }
}
