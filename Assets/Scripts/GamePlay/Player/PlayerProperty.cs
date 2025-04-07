// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_14
// File: PlayerProperty.cs
// Description:
// -------------------------------------------------

using PurpleFlowerCore;
using UnityEngine;

namespace GamePlay.Player
{
    [CreateAssetMenu(fileName = "PlayerProperty", menuName = "Data/PlayerProperty")]
    [Configurable]
    public class PlayerProperty : ScriptableObject
    {
        [Header("公共属性")]
        public float jumpForce = 16f;                       // 跳跃力度
        public int amountOfJump = 1;                        // 跳跃次数（可以连续几段跳）
        public float gravityScale = 5f;                     // 常规情况下重力缩放
        public float commonXMaxSpeed = 10f;                  // 常规情况下最大速度
        public float commonYMaxSpeed = 10f;                  // 常规情况下最大速度
        public float xMaxSpeedRecoverScale = 0.01f;           // x最大速度恢复速度
        public float yMaxSpeedRecoverScale = 0.01f;           // y最大速度恢复速度

        [Header("空中")]
        public float xForceInAir = 10f;                     // 空中水平移动力度
        // public float fallMultiplier = 0.95f;                 // 下落时的空气阻力
        public float variableJumpForce = 0.95f;   // 提前松开空格，则会跳的更低
        
        [Header("悬挂")]
        [Range(0,5)]public float hangDrag = 2f;       //  悬挂且无输入时的空中阻尼
        public float hangSwayForce = 50f;                   // 悬挂时玩家输入的摇摆力
        public float hangGravityScale = 12f;                // 悬挂时的重力缩放
        
        [Header("地面")]
        public float onGroundSpeed = 10f;                   // 地面移动速度
        public float groundCheckHeight = 0.1f;              // 地面检测高度
        public float groundCheckWidth = 0.5f;               // 地面检测宽度
        public LayerMask groundLayer;                       // 地面Layer
        public float jumpTimerSet = 0.15f;                  // 跳跃缓冲时间
        
        [Header("滑墙")]
        public float wallCheckRadius = 0.5f;                // 检测贴墙距离
        public float wallSlideSpeed = 3f;                   // 滑墙速度
        public float wallSpeedRecoverScale = 0.1f;          // 滑墙速度恢复速度
        public float wallJumpForce = 10f;                   // 滑墙跳跃力度
        public float wallJumpTimerSet = 0.15f;              // 滑墙跳跃缓冲时间
        [SerializeField]private Vector2 wallJumpDirection = new(1f, 1f); // 滑墙跳跃方向
        
        [Header("下砸")]
        public float smashVelocity = 30f;               // 下砸速度
        
        [Header("舌头")]
        public float tongueDistance = 8f;
        public float tongueSpeed = 40;
        public float retractSpeed = 100;
        public float minLength = 2;
        
        [Header("攀爬")]
        public float climbSpeed = 5f;                       // 攀爬速度
        
        [Header("其他功能")]
        [SerializeField][Range(0,1)] public float onGroundUpLimit = 0.2f;
        [SerializeField][Range(0,1)] public float onGroundDownLimit = 0.6f;

        private Animator _animator;
        
        // 分类
        public int AmountOfJumpLeft { get; set; }              // 剩余跳跃次数
        public int FacingDirection => IsFacingRight ? 1 : -1;  // _isFacingRight的数值形式，方便计算
        public bool IsFacingRight { get; set; }                // 是否正面向右边

        private bool _isWalking;
        public bool IsWalking
        {
            get => _isWalking;
            set
            {
                if(_isWalking == value) return;
                _isWalking = value;
                _animator.SetBool("Walking", value);
            }
        }

        // 是否在地面上，由Physics2D判定
        private bool _isGrounded;
        public bool IsGrounded
        {
            get => _isGrounded;
            set
            {
                if(_isGrounded == value) return;
                _isGrounded = value;
                _animator.SetBool("Ground", value);
            }
        }

        // 是否与物体连接
        private bool _isConnecting;
        public bool IsConnecting
        {
            get => _isConnecting;
            set
            {
                if(_isConnecting == value) return;
                _isConnecting = value;
                _animator.SetBool("Connecting", value);
            }
        }

        // 是否在滑墙
        private bool _isWallSliding;
        public bool IsWallSliding
        {
            get => _isWallSliding;
            set
            {
                if(_isWallSliding == value) return;
                _isWallSliding = value;
                _animator.SetBool("Wall", value);
            }
        }
        public bool WallJumpFlag { get; set; }                 // 划墙跳后的延迟标记
        public bool IsRightWall { get; set; }                  // 是否在右墙
        public bool CanJump { get; set; }                       // 是否可以进行普通跳跃
        public bool CanMove { get; set; }                      // 是否可以移动
        public bool CanFlip { get; set; }                      // 是否可以转向
        // public float ConnectAngle { get; set; }                // 悬挂时,连接点与玩家的连线与竖直方向的夹角,角度制,当玩家在连接点左侧时为负
        // public Vector3 ConnectDirection { get; set; }                // 悬挂时,连接点与玩家的连线
        public Vector3 HangPoint { get; set; }                  // 悬挂点

        public bool IsOnPillar { get; set; }                   // 是否在可攀爬的柱子前
        public float maxClimbHeight { get; set; }                 // 最大攀爬高度
        public float XMaxSpeed { get; set; }                    // 最大速度
        public float YMaxSpeed { get; set; }                    // 最大速度

        public bool HeadCanMove { get; set; }
        public bool HeadCanLaunch { get; set; }

        
        // todo: 剥离玩家输入

        public Vector2 WallJumpDirection => wallJumpDirection.normalized; // 滑墙跳跃方向
        // [RO] public bool checkVariableJump;                 // 当成功跳跃时被激活，若跳跃期间松开空格，则会施加额外的向下的力
        // [RO] public bool isTouchingWall;                    // 是否贴墙，由Physics2D判定
        
        public void Init(PlayerController player)
        {
            AmountOfJumpLeft = amountOfJump;
            IsFacingRight = true;
            CanMove = true;
            CanFlip = true;
            HeadCanMove = true;
            HeadCanLaunch = true;
            XMaxSpeed = commonXMaxSpeed;
            YMaxSpeed = commonYMaxSpeed;
            _animator = player.Animator;
        }
    }
}