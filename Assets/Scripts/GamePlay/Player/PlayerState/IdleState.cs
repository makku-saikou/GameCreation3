// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_15
// File: IdleState.cs
// Description:
// -------------------------------------------------

using System;
using Common.FSM;

namespace GamePlay.Player.PlayerState
{
    [Obsolete("暂时不引入子状态")]
    public class IdleState : OnGround
    {

        public override void EnterCallback(HState prev)
        {
            base.EnterCallback(prev);
            
        }

        public IdleState(string name, PlayerProperty property) : base(name, property)
        {
        }

        public IdleState(PlayerProperty property) : base(property)
        {
        }
    }
}