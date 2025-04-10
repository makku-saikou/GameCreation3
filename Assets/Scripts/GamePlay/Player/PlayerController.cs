// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_08
// File: PlayerController.cs
// Description: 玩家角色主要控制逻辑
// -------------------------------------------------

using Common.FSM;
using GamePlay.Player.PlayerInput;
using GamePlay.Player.PlayerState;
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

        [SerializeField] private PlayerProperty property;
        public PlayerProperty Property => property;

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

        private void Awake()
        {
            Init();
        }

        private void Update()
        {
            CheckState();
            StateMachine.UpdateCallback(Time.deltaTime);
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
            property.Init(this);
            
            //todo:
            _input = new PlayerInput_Legacy();
            
            // 定义整个状态机
            // 状态
            OnGroundState onGroundState = new OnGroundState(this, "OnGround");
            AirState airState = new AirState(this, "Air");
            HangState hangState = new HangState(this, "Hang");
            OnWallState onWallState = new OnWallState(this, "OnWall");
            SmashState smashState = new SmashState(this, "Smash");
            OnPillarState onPillarState = new OnPillarState(this, "OnPillar");
            
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
            onWall.OnCheck += () => property.IsWallSliding && !property.WallJumpFlag;
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
            climb.OnCheck += () => property.IsOnPillar && Input.UpInput;
            onGroundState.AddTransition(climb);
            
            HTransition climbLand = new HTransition("ClimbLand", onPillarState, onGroundState);
            climbLand.OnCheck += () => property.IsOnPillar && Input.DownInput && property.IsGrounded;
            onPillarState.AddTransition(climbLand);
            
            HTransition climbJump = new HTransition("ClimbJump", onPillarState, airState);
            climbJump.OnCheck += () => property.IsOnPillar && Input.JumpInputDown;
            onPillarState.AddTransition(climbJump);
            
            HTransition climbToHang = new HTransition("ClimbToHang", onPillarState, hangState);
            climbToHang.OnCheck += () => property.IsOnPillar && property.IsConnecting;
            onPillarState.AddTransition(climbToHang);
            
            HTransition airClimb = new HTransition("AirClimb", airState, onPillarState);
            airClimb.OnCheck += () => property.IsOnPillar && Input.UpInput;
            airState.AddTransition(airClimb);
            
            // 初始化状态机
            _stateMachine = new HStateMachine(onGroundState);
            _stateMachine.AddState(airState);
            _stateMachine.AddState(hangState);
            _stateMachine.AddState(onWallState);
            _stateMachine.AddState(smashState);
            _stateMachine.AddState(onPillarState);
        }
        
        /// <summary>
        /// 检查状态的替换由玩家本体负责，而不放入状态中
        /// </summary>
        private void CheckState()
        {
            property.IsGrounded = 
                Physics2D.OverlapBox(groundCheckPoint.position, new Vector2(property.groundCheckWidth,
                    property.groundCheckHeight), 0, property.groundLayer);
            bool rightWall =
                Physics2D.OverlapCircle(wallCheckPoint2.position, property.wallCheckRadius, property.groundLayer);
            property.IsRightWall = rightWall;
            property.IsWallSliding = Physics2D.OverlapCircle(wallCheckPoint1.position, property.wallCheckRadius,
                property.groundLayer) || rightWall;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Pillar"))
            {
                property.IsOnPillar = true;
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Pillar"))
            {
                property.IsOnPillar = false;
            }
        }
        
        private void RecoverMaxSpeed()
        {
            property.XMaxSpeed = Mathf.Lerp(property.XMaxSpeed, property.commonXMaxSpeed,
                property.xMaxSpeedRecoverScale);
            property.YMaxSpeed = Mathf.Lerp(property.YMaxSpeed, property.commonYMaxSpeed,
                property.yMaxSpeedRecoverScale);
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(groundCheckPoint.position, new Vector3(property.groundCheckWidth, property.groundCheckHeight, 0));
            Gizmos.DrawWireSphere(wallCheckPoint1.position, property.wallCheckRadius);
            Gizmos.DrawWireSphere(wallCheckPoint2.position, property.wallCheckRadius);
        }
    }
}