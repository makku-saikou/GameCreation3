// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_04_19
// Description:
// -------------------------------------------------

using UnityEngine;
using Common.FSM;
using PurpleFlowerCore;

namespace GamePlay.Player.PlayerState
{
    public class OnBackgroundState : PlayerStateBase
    {
        public OnBackgroundState(PlayerController player, string name) : base(player, name) { }
        
        public override void EnterCallback(HState prev)
        {
            base.EnterCallback(prev);
            PFCLog.Debug("Enter OnBackground State");
            Player.Head.SetShow(false);
            Rb.gravityScale = 0;
            Rb.velocity = Vector2.zero;
            Property.IsOnColorBlock = true;
        }

        public override void FixedUpdateCallback()
        {
            base.FixedUpdateCallback();
            var direction = Input.DirectionInput;
            if (direction != Vector2.zero)
            {
                // direction = Vector3.Lerp(Player.Entity.up, direction, 0.01f);
                // Rb.velocity = new Vector2(direction.x * Config.climbBackgroundSpeed, direction.y * Config.climbBackgroundSpeed);
                Player.transform.position += new Vector3(direction.x , direction.y) * (Config.climbBackgroundSpeed * Time.fixedDeltaTime);
                Player.Entity.up = direction;
            }
        }

        public override void ExitCallback(HState next)
        {
            base.ExitCallback(next);
            PFCLog.Debug("Exit OnBackground State");
            Player.Head.SetShow(true);
            Rb.gravityScale = Config.gravityScale;
            Property.IsOnColorBlock = false;
        }
    }
}