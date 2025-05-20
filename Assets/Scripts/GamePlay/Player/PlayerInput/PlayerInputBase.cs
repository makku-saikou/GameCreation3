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

        protected PlayerController Player;
        protected PlayerInputBase(PlayerController player)
        {
            Player = player;
        }
    }
}
