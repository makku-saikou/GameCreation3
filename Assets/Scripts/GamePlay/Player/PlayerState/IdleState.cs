// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_15
// File: IdleState.cs
// Description:
// -------------------------------------------------

using System;

namespace GamePlay.Player.PlayerState
{
    [Obsolete("暂时不引入子状态")]
    public class IdleState : PlayerStateBase
    {
        public IdleState(PlayerController player, string name) : base(player, name) { }
    }
}