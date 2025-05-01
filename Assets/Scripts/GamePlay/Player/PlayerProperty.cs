// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_05_01
// Description:
// -------------------------------------------------

using System;
using PurpleFlowerCore.Utility;
using UnityEngine;

namespace GamePlay.Player
{
    public class PlayerProperty
    {
        private PlayerController _player;
        private Animator Animator => _player.Animator;
        private Rigidbody2D Rb => _player.Rb;
        private PlayerConfig Config => _player.Config;
        
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
                Animator.SetBool("Walking", value);
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
                Animator.SetBool("Ground", value);
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
                Animator.SetBool("Connecting", value);
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
                    Animator.SetBool("Wall", false);
                else 
                {
                    DelayUtility.Delay(0.02f, () =>
                    {
                        if (_isWallSliding)
                            Animator.SetBool("Wall", true);
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
                Animator.SetBool("OnWallFlag", value);
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
                Animator.SetBool("OnBackground", value);
            }
        }
        public float MaxClimbHeight { get; set; }              // 最大攀爬高度
        public float XMaxSpeed { get; set; }                   // 最大速度
        public float YMaxSpeed { get; set; }                    // 最大速度
        public bool HeadCanMove { get; set; }
        public bool HeadCanLaunch { get; set; }
        public float CurrentTongueLength { get; set; }       // 舌头当前长度
        
        public Vector2 WallJumpDirection => Config.wallJumpDirection.normalized; // 滑墙跳跃方向
        // [RO] public bool checkVariableJump;                 // 当成功跳跃时被激活，若跳跃期间松开空格，则会施加额外的向下的力
        // [RO] public bool isTouchingWall;                    // 是否贴墙，由Physics2D判定

        public bool IsInCannon { get; set; } // 是否在炮筒里

        public PlayerProperty(PlayerController player)
        {
            _player = player;
            AmountOfJumpLeft = Config.amountOfJump;
            IsFacingRight = true;
            CanMove = true;
            CanFlip = true;
            HeadCanMove = true;
            HeadCanLaunch = true;
            XMaxSpeed = Config.commonXMaxSpeed;
            YMaxSpeed = Config.commonYMaxSpeed;
        }

        public void ResetWallJumpTimer()
        {
            OnWallFlag = true;
            DelayUtility.Delay(Config.wallJumpTimerSet, () => { OnWallFlag = false; });
        }
    }
}