// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_20
// Description:
// -------------------------------------------------

using Common.FSM;
using PurpleFlowerCore;
using PurpleFlowerCore.Resource;
using UnityEngine;

namespace GamePlay.Player.PlayerState
{
    public class HangState : PlayerStateBase
    {
        public HangState(PlayerController player, string name) : base(player, name) { }
        public override void EnterCallback(HState prev)
        {
            base.EnterCallback(prev);
            PFCLog.Debug("Enter Hang State");
            Rb.gravityScale = P.hangGravityScale;
        }

        public override void UpdateCallback(float deltaTime)
        {
            base.UpdateCallback(deltaTime);

            if (Input.JumpInputDown)
            {
                // todo: 解耦
                Player.Head.RetractTongue();
                HangJump();
            }
        }

        public override void FixedUpdateCallback()
        {
            base.FixedUpdateCallback();
                
            Player.Rb.drag = Input.MovementInput == 0 ? P.hangDrag : 0;
            if (Input.MovementInput != 0)
            {
                Player.Rb.AddForce(new Vector2(P.hangSwayForce * Input.MovementInput, 0), ForceMode2D.Force);
            }
        }

        public override void ExitCallback(HState next)
        {
            base.ExitCallback(next);
            Rb.gravityScale = P.gravityScale;
            Player.Rb.drag = 0;
            P.XMaxSpeed = Mathf.Abs(P.XMaxSpeed);
            P.YMaxSpeed = Mathf.Abs(P.YMaxSpeed);
        }

        private void HangJump()
        {
            Rb.velocity += new Vector2(0, P.jumpForce);
        }
    }
}