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
            Player.Head.SetShow(false);
        }

        public override void ExitCallback(HState next)
        {
            base.ExitCallback(next);
            PFCLog.Debug("Exit Air State");
            Player.Head.SetShow(true);
        }
        
        public override void FixedUpdateCallback()
        {
            base.FixedUpdateCallback();
            
            if (Input.MovementInput != 0)
            {
                Rb.AddForce(new Vector2(Property.xForceInAir * Input.MovementInput, 0), ForceMode2D.Force);
            }
            var velocity = Rb.velocity;
            
            if (Mathf.Abs(velocity.x) > Property.XMaxSpeed)
            {
                velocity = new Vector2(Mathf.Sign(velocity.x) * Property.XMaxSpeed, velocity.y);
            }
            if (Mathf.Abs(velocity.y) > Property.YMaxSpeed)
            {
                velocity = new Vector2(velocity.x, Mathf.Sign(velocity.y) * Property.YMaxSpeed);
            }
            if (!Input.JumpInput)
            {
                velocity = new Vector2(velocity.x, velocity.y - Property.variableJumpForce);
            }
            Rb.velocity = velocity;
        }
    }
}