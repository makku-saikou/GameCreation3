// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_3_24
// Description:
// -------------------------------------------------

using Common.FSM;
using UnityEngine;

namespace GamePlay.Player.PlayerState
{
    public class SmashState : PlayerStateBase
    {
        public SmashState(PlayerController player, string name) : base(player, name) {}
        
        public override void EnterCallback(HState prev)
        {
            base.EnterCallback(prev);
            Player.Head.SetShow(false);
        }
        
        public override void ExitCallback(HState next)
        {
            base.ExitCallback(next);
            Player.Head.SetShow(true);
        }

        public override void FixedUpdateCallback()
        {
            base.FixedUpdateCallback();
            Rb.velocity = new Vector2(0, -Property.smashVelocity);
        }
    }
}