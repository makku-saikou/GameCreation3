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
    public class OnPillarState : PlayerStateBase
    {
        public OnPillarState(PlayerController player, string name) : base(player, name) { }
        
        public override void EnterCallback(HState prev)
        {
            base.EnterCallback(prev);
            Rb.gravityScale = 0;
            Rb.velocity = Vector2.zero;
        }

        public override void FixedUpdateCallback()
        {
            base.FixedUpdateCallback();
            if(Input.UpInput && Player.transform.position.y < Property.maxClimbHeight)
            {
                Rb.velocity = new Vector2(0, Property.climbSpeed);
            }
            else if(Input.DownInput)
            {
                Rb.velocity = new Vector2(0, -Property.climbSpeed);
            }
            else
            {
                Rb.velocity = new Vector2(Rb.velocity.x, 0);
            }
        }

        public override void ExitCallback(HState next)
        {
            base.ExitCallback(next);
            Rb.gravityScale = Property.gravityScale;
        }
    }
}