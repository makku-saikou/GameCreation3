// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_3_24
// Description:
// -------------------------------------------------

using UnityEngine;

namespace GamePlay.Player.PlayerState
{
    public class SmashState : PlayerStateBase
    {
        public SmashState(PlayerController player, string name) : base(player, name) {}

        public override void FixedUpdateCallback()
        {
            base.FixedUpdateCallback();
            _rb.velocity = new Vector2(0, -_p.smashVelocity);
        }
    }
}