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
    // todo: 虽然玩家属性越来越屎，但我懒得系统设计 :(
    public class PlayerProperty
    {
        private readonly PlayerController _player;
        private Animator Animator => _player.Animator;
        private Rigidbody2D Rb => _player.Rb;
        private PlayerConfig Config => _player.Config;
        
        // 分类
        public int AmountOfJumpLeft { get; set; }           // 剩余跳跃次数
        public int FacingDirection => IsFacingRight ? 1 : -1;// _isFacingRight的数值形式，方便计算
        public bool IsFacingRight { get; set; }             // 是否正面向右边

        // 是否在地面上，由Physics2D判定
        public bool IsGrounded { get; set; } = true;
        // 是否与物体连接
        public bool IsConnecting { get; set; }

        // 是否在滑墙
        public bool IsNearWall { get; set; }
        // todo: 直接获取舌头状态
        public bool IsLaunching { get; set; }
        public bool IsRetracting { get; set; }

        // 划墙跳后的延迟标记
        public bool OnWallFlag { get; set; }
        
        // 悬挂时，当前舌头与竖直方向的夹角，-90~90,玩家在右边时为正
        private float _currentHongAngle;
        public float CurrentHongAngle                              
        {
            get => _currentHongAngle;
            set
            {
                _currentHongAngle = value;
                var clamp = Mathf.Clamp(_currentHongAngle, -90, 90);
                float progress = FacingDirection * clamp / 91f * 0.5f + 0.5f;
                if(_player.CurrentStateName == "Hang")
                    Animator.Play( "Hang", 0, progress);
            }
        }
        private bool _isAirLaunching;

        public bool IsAirLaunching
        {
            get => _isAirLaunching;
            set
            {
                if(_isAirLaunching == value) return;
                _isAirLaunching = value;
                Animator.SetBool("AirLaunch", value);
                _player.EntityBackground.enabled = value;
            }
        }

        // private bool _isWalking;
        // public bool IsWalking
        // {
        //     get => _isWalking;
        //     set
        //     {
        //         if (_isWalking == value) return;
        //         _isWalking = value;
        //         Animator.SetBool("Walk", _isWalking);
        //     }
        // }
        
        private bool _aniMove;
        public bool AniMove
        {
            get => _aniMove;
            set
            {
                _aniMove = value;
                Animator.enabled = _aniMove;
            }
        }
        
        public bool IsRightWall { get; set; }                  // 是否在右墙
        public bool CanJump { get; set; }                      // 是否可以进行普通跳跃
        public bool JumpBufferFlag { get; set; }               // 是否可以进行更高跳跃 
        public bool CanMove { get; set; }                      // 是否可以移动
        public bool CanFlip { get; set; }                      // 是否可以转向
        public Vector3 HangPoint => _player.Head.Tongue.TargetPosition;
        public bool CanOnPillar { get; set; }                  // 是否在可攀爬的柱子前
        public bool CanOnSwimColorBlock { get; set; }              // 是否在色块前
        
        private float _currentColorDuration;
        
        private EPlayerColor _currentColor;

        public EPlayerColor CurrentColor
        {
            get => _currentColor;
            set
            {
                if (_currentColor == value) return;
                var oldColor = _currentColor;
                _currentColor = value;
                if (value != EPlayerColor.None)
                {
                    _currentColorDuration = Config.colorDuration;
                }
                OnColorChanged?.Invoke(oldColor, _currentColor);
            }
        }
        public event Action<EPlayerColor, EPlayerColor> OnColorChanged;
        
        public bool PreJumpBufferFlag { get; set; }          // 在地面时直接进行跳跃
        public Vector2 MaxClimbHeight { get; set; }              // 攀爬最高点
        public bool HeadCanMove { get; set; }
        
        private bool _headCanLaunch;
        public bool HeadCanLaunch
        {
            get => _headCanLaunch;
            set
            {
                HeadCanLaunch = value;
                // _player.Head.Tongue
            }
        }
        public float CurrentTongueLength { get; set; }       // 舌头当前长度
        
        public Vector2 WallJumpDirection => Config.wallJumpDirection.normalized; // 滑墙跳跃方向

        public bool IsInCannon { get; set; } // 是否在炮筒里
        
        public bool IsShuttle { get; set; } // 是否在穿梭色块里
        
        public bool SmashFlag { get; set; } // 是否可以下砸
        
        public bool ClimbFlag { get; set; }
        public bool HasSmashLanded { get; set; } // 是否已经下砸到地面过

        public PlayerProperty(PlayerController player)
        {
            _player = player;
            AmountOfJumpLeft = Config.amountOfJump;
            IsFacingRight = true;
            CanMove = true;
            CanFlip = true;
            HeadCanMove = true;
            HeadCanLaunch = true;
            SmashFlag = true;
            ClimbFlag = true;
        }

        public void Update(float deltaTime)
        {
            _currentColorDuration -= deltaTime;
            if(_currentColorDuration <= 0 && CurrentColor != EPlayerColor.None)
            {
                CurrentColor = EPlayerColor.None;
            }
        }

        public void FixedUpdate(float deltaTime)
        {
            
        }

        public void ResetWallJumpTimer()
        {
            OnWallFlag = true;
            DelayUtility.Delay(Config.wallJumpTimerSet, () => { OnWallFlag = false; });
        }
        
        public void ResetPreJumpBufferFlag()
        {
            PreJumpBufferFlag = true;
            DelayUtility.Delay(Config.preJumpBufferTime, () => { PreJumpBufferFlag = false; });
        }
        
        public void ResetJumpBufferFlag()
        {
            JumpBufferFlag = true;
            DelayUtility.Delay(Config.jumpBufferTime, () => { JumpBufferFlag = false; });
        }
    }
}