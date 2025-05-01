// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_3_20
// Description:
// -------------------------------------------------

using Common.FSM;
using PurpleFlowerCore;
using UnityEngine;

namespace GamePlay.Player.PlayerState
{
    public class OnWallState : PlayerStateBase
    {
        public OnWallState(PlayerController player, string name) : base(player, name) { }
        
        public override void EnterCallback(HState prev)
        {
            base.EnterCallback(prev);
            Rb.gravityScale = 0;

            Vector2 vector = Rb.velocity;
            vector.x = 0;
            Rb.velocity = vector;
            
            Player.Head.SetShow(false);
            Property.HeadCanLaunch = false;
        }

        public override void ExitCallback(HState next)
        {
            base.ExitCallback(next);
            Property.ResetWallJumpTimer();
            Rb.gravityScale = Config.gravityScale;
            
            Player.Head.SetShow(true);
            Property.HeadCanLaunch = true;
        }

        public override void UpdateCallback(float deltaTime)
        {
            base.UpdateCallback(deltaTime);
            if (Input.JumpInputDown)
            {
                PFCLog.Debug("Wall Jump");
                Vector2 direction = Property.WallJumpDirection;
                if (Property.IsRightWall)
                {
                    direction.x = -direction.x;
                }
                Rb.AddForce(direction * Config.wallJumpForce, ForceMode2D.Impulse);
                Property.ResetWallJumpTimer();
            }
        }

        public override void FixedUpdateCallback()
        {
            base.FixedUpdateCallback();
            
            Vector2 velocity = Rb.velocity;
            // velocity = Vector2.Lerp(velocity, new Vector2(0, 0), P.wallSpeedRecoverScale);
            velocity = Vector2.Lerp(velocity, new Vector2(0, -Config.wallSlideSpeed), Config.wallSpeedRecoverScale);
            Rb.velocity = velocity;
        }
    }
}