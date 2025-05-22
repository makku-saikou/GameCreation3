// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_3_31
// Description:
// -------------------------------------------------

using UnityEngine;

namespace GamePlay.Player.PlayerInput
{
    public abstract class PlayerInputBase
    {
        public abstract float MovementInput { get; } // x输入
        public abstract bool JumpInputDown { get; } // 输入跳跃按下
        public abstract bool JumpInput { get; } // 输入跳跃按住
        public abstract bool DownInput { get; } // 输入下砸
        public abstract bool UpInput { get; } // 输入上
        public abstract Vector2 DirectionInput { get; } // 输入二维方向
        public abstract bool InteractInput { get; } // 输入交互
        public abstract Vector2 AttentionDirection { get; }
        
        public abstract bool LaunchDown { get; }
        public abstract bool LaunchUp { get; }
        public abstract bool ConnectInteractDown { get; }

        /// <summary>
        /// X输入方向的“程度”，左负右正，按下时间越长，值越大
        /// </summary>
        public float XInputExtent;

        protected PlayerController Player;
        
        private float _lastMovementInput;
        protected PlayerInputBase(PlayerController player)
        {
            Player = player;
        }

        public virtual void FixedUpdate()
        {
            if(MovementInput * _lastMovementInput <= 0)
            {
                XInputExtent = 0;
            }
            else
            {
                XInputExtent += MovementInput;
            }
            _lastMovementInput = MovementInput;
        }
    }
}
