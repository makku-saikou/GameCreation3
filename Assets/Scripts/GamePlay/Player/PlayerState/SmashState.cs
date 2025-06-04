// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_3_24
// Description:
// -------------------------------------------------

using Common.FSM;
using PurpleFlowerCore.Utility;
using UnityEngine;

namespace GamePlay.Player.PlayerState
{
    public class SmashState : PlayerStateBase
    {
        public SmashState(PlayerController player, string name) : base(player, name) {}
        
        public override void EnterCallback(HState prev)
        {
            base.EnterCallback(prev);
            Property.HasSmashLanded = false;
            Player.Head.SetShow(false);
            Property.HeadCanLaunch = false;
            Player.AddGravityEffect("Smash", Config.smashGravityScale, Config.smashGravityScaleTime);
            DelayUtility.Delay(Config.smashGravityScaleTime, () =>
            {
                Rb.velocity = new Vector2(Rb.velocity.x, -Config.smashVelocity);
            });
        }

        public override void LateUpdateCallback(float deltaTime)
        {
            base.LateUpdateCallback(deltaTime);
            if (Property.IsGrounded)
            {
                Bounce();
                DelayUtility.Delay(0.1f, () =>
                {
                    Property.HasSmashLanded = true;
                });
            }

        }

        public override void ExitCallback(HState next)
        {
            base.ExitCallback(next);
            Property.HasSmashLanded = false;
            Player.Head.SetShow(true);
            Property.HeadCanLaunch = true;
            Property.SmashFlag = false;
            DelayUtility.Delay(Config.smashCD, () =>
            {
                Property.SmashFlag = true;
            });
        }
        
        private void Bounce()
        {
            // Rb.AddForce(Vector2.up * Config.smashBounceForce, ForceMode2D.Impulse);
            Rb.velocity = new Vector2(Rb.velocity.x, Config.smashBounceForce);
        }
    }
}