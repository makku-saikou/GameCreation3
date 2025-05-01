// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_14
// File: PlayerProperty.cs
// Description:
// -------------------------------------------------

using System;
using System.Collections.Generic;
using Common.Attribute;
using PurpleFlowerCore;
using PurpleFlowerCore.Utility;
using UnityEngine;

namespace GamePlay.Player
{
    /// <summary>
    /// 玩家属性的配置类和数据类，我们使用字段配置数据，使用属性缓存和传递信息
    /// 同时，我们使用属性来封装和控制动画的播放
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerProperty", menuName = "Data/PlayerProperty")]
    [Configurable("Player")]
    public class PlayerProperty : ScriptableObject
    {
        [Header("公共属性")]
        [Comment("常规情况跳跃力度")]public float jumpForce = 20f; 
        [Comment("跳跃次数（可以连续几段跳）")]public int amountOfJump = 1;                        
        [Comment("常规情况下重力缩放")]public float gravityScale = 5f;                     
        [Comment("常规情况下最大速度")]public float commonXMaxSpeed = 10f;                 
        [Comment("常规情况下最大速度")]public float commonYMaxSpeed = 60f;                 
        [Comment("x最大速度插值恢复比率")]public float xMaxSpeedRecoverScale = 0.01f;         
        [Comment("y最大速度插值恢复比率")]public float yMaxSpeedRecoverScale = 0.05f;         

        [Header("空中")]
        [Comment("空中水平移动力度")]public float xForceInAir = 200f;                     
        // public float fallMultiplier = 0.95f;             // 下落时的空气阻力
        [Comment("提前松开空格，则会跳的更低")]public float variableJumpForce = 0.95f;             
        
        [Header("悬挂")]
        [Comment("悬挂且无输入时的空中阻尼")]public float hangDrag = 2f;             
        [Comment("悬挂时玩家输入的摇摆力")]public float hangSwayForce = 100f;                   
        [Comment("悬挂时的重力缩放")]public float hangGravityScale = 12f;                
        [Comment("ws时舌头长度变化速度")]public float tongueLengthChangeSpeed = 1f;
        [Comment("发射时玩家悬停速度缩放")]public float launchDragScale = 0.3f;
        [Comment("悬挂时的跳跃力度")]public float hangJumpForce = 20f;
        
        [Header("地面")]
        [Comment("地面移动速度")]public float onGroundSpeed = 10f;                   
        [Comment("地面检测高度")]public float groundCheckHeight = 0.1f;              
        [Comment("地面检测宽度")]public float groundCheckWidth = 0.5f;               
        [Comment("地面Layer")]public LayerMask groundLayer;                       
        [Comment("跳跃缓冲时间")]public float jumpTimerSet = 0.15f;                  
        [Comment("跳跃后在一定时间内按跳跃可以跳的更高")] public float jumpBufferTime = 0.5f;
        
        [Header("扒墙")]
        [Comment("检测贴墙距离")]public float wallCheckRadius = 0.1f;                
        [Comment("滑墙速度")]public float wallSlideSpeed = 3f;                   
        [Comment("滑墙速度插值恢复比率")]public float wallSpeedRecoverScale = 0.15f;          
        [Comment("蹬墙跳跃力度")]public float wallJumpForce = 120f;                    
        [Comment("蹬墙跳跃缓冲时间，在该时间内不会再次进入扒墙状态")]public float wallJumpTimerSet = 0.15f;              
        [Comment("蹬墙跳跃方向")][SerializeField]private Vector2 wallJumpDirection = new(1f, 1f);
        
        [Header("下砸")]
        [Comment("下砸下降速度")]public float smashVelocity = 30f;                   
        
        [Header("舌头")]
        [Comment("舌头发射速度")]public float tongueSpeed = 40;
        [Comment("舌头回到嘴的速度")]public float retractSpeed = 100;
        [Comment("舌头最大长度,影响射程和悬挂时的最大长度")]public float tongueMaxLength = 8f;
        [Comment("舌头最小长度,影响时的最小长度")]public float tongueMinLength = 2;
        [Comment("舌头可以碰撞到的layer")] public List<LayerMask> targetLayers;
        
        [Header("爬杆")]
        [Comment("爬杆攀爬速度")]public float climbPileSpeed = 5f;                       
        
        [Header("爬背景墙")]
        [Comment("爬背景墙速度")]public float climbBackgroundSpeed = 10f;
        
        [Header("其他功能")]
        [Comment("地面上时最大抬头角度")][SerializeField] public float onGroundUpLimit = 0.2f;
        [Comment("地面上时最大低头角度")][SerializeField] public float onGroundDownLimit = 0.6f;

        private Animator _animator;
        
        
        // todo: 剥离非配置的运行时信息
        // 分类
        public int AmountOfJumpLeft { get; set; }           // 剩余跳跃次数
        public int FacingDirection => IsFacingRight ? 1 : -1;// _isFacingRight的数值形式，方便计算
        public bool IsFacingRight { get; set; }             // 是否正面向右边

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
                if(!value)
                    _animator.SetBool("Wall", false);
                else 
                {
                    DelayUtility.Delay(0.02f, () =>
                    {
                        if (_isWallSliding)
                            _animator.SetBool("Wall", true);
                    });
                }
            }
        }
        
        private bool _isLaunching;
        public bool IsLaunching
        {
            get => _isLaunching;
            set
            {
                if (_isLaunching == value) return;
                _isLaunching = value;
                // todo: 发射时的表现
                // _animator.SetBool("Launch", value);
            }
        }
        
        private bool _isRetracting;
        public bool IsRetracting
        {
            get => _isRetracting;
            set
            {
                if (_isRetracting == value) return;
                _isRetracting = value;
                // _animator.SetBool("Retract", value);
            }
        }
        // 划墙跳后的延迟标记
        private bool _onOnWallFlag;
        public bool OnWallFlag
        {
            get => _onOnWallFlag;
            set
            {
                if (_onOnWallFlag == value) return;
                _onOnWallFlag = value;
                _animator.SetBool("OnWallFlag", value);
            }
        }                 
        public bool IsRightWall { get; set; }                  // 是否在右墙
        public bool CanJump { get; set; }                      // 是否可以进行普通跳跃
        public bool JumpBufferFlag { get; set; }               // 是否可以进行更高跳跃 
        public bool CanMove { get; set; }                      // 是否可以移动
        public bool CanFlip { get; set; }                      // 是否可以转向
        // public float ConnectAngle { get; set; }             // 悬挂时,连接点与玩家的连线与竖直方向的夹角,角度制,当玩家在连接点左侧时为负
        // public Vector3 ConnectDirection { get; set; }        // 悬挂时,连接点与玩家的连线
        public Vector3 HangPoint { get; set; }                  // 悬挂点
        public bool CanOnPillar { get; set; }                   // 是否在可攀爬的柱子前
        public bool IsOnPillar { get; set; }                    // 是否正在爬柱子
        public bool CanOnColorBlock { get; set; }               // 是否在色块前
        
        private EPlayerColor _currentColor;

        public EPlayerColor CurrentColor
        {
            get => _currentColor;
            set
            {
                if (_currentColor == value) return;
                _currentColor = value;
                OnColorChanged?.Invoke();
            }
        }
        public event Action OnColorChanged;


        // 是否正在色块背景上爬
        private bool _isOnColorBlock;
        public bool IsOnColorBlock
        {
            get => _isOnColorBlock;
            set
            {
                if (_isOnColorBlock == value) return;
                _isOnColorBlock = value;
                _animator.SetBool("OnBackground", value);
            }
        }
        public float MaxClimbHeight { get; set; }              // 最大攀爬高度
        public float XMaxSpeed { get; set; }                   // 最大速度
        public float YMaxSpeed { get; set; }                    // 最大速度
        public bool HeadCanMove { get; set; }
        public bool HeadCanLaunch { get; set; }
        public float CurrentTongueLength { get; set; }       // 舌头当前长度
        
        public Vector2 WallJumpDirection => wallJumpDirection.normalized; // 滑墙跳跃方向
        // [RO] public bool checkVariableJump;                 // 当成功跳跃时被激活，若跳跃期间松开空格，则会施加额外的向下的力
        // [RO] public bool isTouchingWall;                    // 是否贴墙，由Physics2D判定

        public bool IsInCannon { get; set; } // 是否在炮筒里
        
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

        public void ResetWallJumpTimer()
        {
            OnWallFlag = true;
            DelayUtility.Delay(wallJumpTimerSet, () => { OnWallFlag = false; });
        }
    }
}
