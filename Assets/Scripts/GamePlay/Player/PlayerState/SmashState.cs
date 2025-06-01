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
            Player.Head.SetShow(false);
            Property.HeadCanLaunch = false;
            Player.AddGravityEffect("Smash", Config.smashGravityScale, Config.smashGravityScaleTime);
            DelayUtility.Delay(Config.smashGravityScaleTime, () =>
            {
                Rb.velocity = new Vector2(0, -Config.smashVelocity);
            });
        }
        
        public override void ExitCallback(HState next)
        {
            base.ExitCallback(next);
            Player.Head.SetShow(true);
            Property.HeadCanLaunch = true;
            
            Rb.AddForce(Vector2.up * Config.smashBounceForce, ForceMode2D.Impulse);
            
            Property.SmashFlag = false;
            DelayUtility.Delay(Config.smashCD, () =>
            {
                Property.SmashFlag = true;
            });
        }
    }
}