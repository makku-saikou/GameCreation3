// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_3_20
// Description:
// -------------------------------------------------

using Common.FSM;
using UnityEngine;

namespace GamePlay.Player.PlayerState
{
    public class OnWallState : PlayerStateBase
    {
        public OnWallState(PlayerController player, string name) : base(player, name) { }
        
        public override void EnterCallback(HState prev)
        {
            base.EnterCallback(prev);
            _rb.gravityScale = 0;
        }

        public override void ExitCallback(HState next)
        {
            base.ExitCallback(next);
            _rb.gravityScale = _p.gravityScale;
        }
        
        public override void FixedUpdateCallback()
        {
            base.FixedUpdateCallback();
            _rb.velocity = new Vector2(0, -_p.wallSlideSpeed);
        }
    }
}