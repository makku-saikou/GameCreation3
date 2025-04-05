// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_16
// Description:
// -------------------------------------------------

using Common.FSM;
using PurpleFlowerCore;
using UnityEngine;

namespace GamePlay.Player.PlayerState
{
    public class AirState : PlayerStateBase
    {
        public AirState(PlayerController player, string name) : base(player, name) { }

        public override void EnterCallback(HState prev)
        {
            base.EnterCallback(prev);
            PFCLog.Debug("Enter Air State");
        }

        public override void ExitCallback(HState next)
        {
            base.ExitCallback(next);
            PFCLog.Debug("Exit Air State");
        }
        
        public override void FixedUpdateCallback()
        {
            base.FixedUpdateCallback();
            
            if (Input.MovementInput != 0)
            {
                Rb.AddForce(new Vector2(P.xForceInAir * Input.MovementInput, 0), ForceMode2D.Force);
            }
            var velocity = Rb.velocity;
            
            if (Mathf.Abs(velocity.x) > P.XMaxSpeed)
            {
                velocity = new Vector2(Mathf.Sign(velocity.x) * P.XMaxSpeed, velocity.y);
            }
            if (Mathf.Abs(velocity.y) > P.YMaxSpeed)
            {
                velocity = new Vector2(velocity.x, Mathf.Sign(velocity.y) * P.YMaxSpeed);
            }
            if (!Input.JumpInput)
            {
                velocity = new Vector2(velocity.x, velocity.y - P.variableJumpForce);
            }
            Rb.velocity = velocity;
        }
        
        
    }
}