// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_3_20
// Description:
// -------------------------------------------------

using Common.FSM;
using PurpleFlowerCore;
using PurpleFlowerCore.Utility;
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
        }

        public override void ExitCallback(HState next)
        {
            base.ExitCallback(next);
            Rb.gravityScale = P.gravityScale;
        }

        public override void UpdateCallback(float deltaTime)
        {
            base.UpdateCallback(deltaTime);
            if (Input.JumpInputDown)
            {
                PFCLog.Debug("Wall Jump");
                Vector2 direction = P.WallJumpDirection;
                if (P.IsRightWall)
                {
                    direction.x = -direction.x;
                }
                Rb.AddForce(direction * P.wallJumpForce, ForceMode2D.Impulse);
                P.WallJumpFlag = true;
                DelayUtility.Delay(P.wallJumpTimerSet, () =>
                {
                    P.WallJumpFlag = false;
                });
            }
        }

        public override void FixedUpdateCallback()
        {
            base.FixedUpdateCallback();
            
            Vector2 velocity = Rb.velocity;
            velocity = Vector2.Lerp(velocity, new Vector2(0, -P.wallSlideSpeed), P.wallSpeedRecoverScale);
            Rb.velocity = velocity;
        }
    }
}