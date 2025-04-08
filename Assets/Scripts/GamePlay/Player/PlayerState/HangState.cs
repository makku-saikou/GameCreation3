// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_20
// Description:
// -------------------------------------------------

using Common.FSM;
using PurpleFlowerCore;
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
            Rb.gravityScale = Property.hangGravityScale;
            Player.Head.SetShow(false);
        }
        
        public override void ExitCallback(HState next)
        {
            base.ExitCallback(next);
            Rb.gravityScale = Property.gravityScale;
            Player.Rb.drag = 0;
            Property.XMaxSpeed = Mathf.Abs(Property.XMaxSpeed);
            Property.YMaxSpeed = Mathf.Abs(Property.YMaxSpeed);
            Player.Head.SetShow(true);
            
            Player.transform.right = Vector2.right;
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
            ChangeTongueLength();
            BodyDirection();
            Move();
        }
        
        private void HangJump()
        {
            Rb.velocity += new Vector2(0, Property.jumpForce);
        }

        private void BodyDirection()
        {
            var direction = Property.HangPoint - Player.transform.position;
            direction.Normalize();
            Player.transform.up = direction;
        }

        private void Move()
        {
            Player.Rb.drag = Input.MovementInput == 0 ? Property.hangDrag : 0;
            if (Input.MovementInput != 0)
            {
                Player.Rb.AddForce(new Vector2(Property.hangSwayForce * Input.MovementInput, 0), ForceMode2D.Force);
            }
        }

        private void ChangeTongueLength()
        {
            if (Input.UpInput)
            {
                Property.CurrentTongueLength -= Property.tongueLengthChangeSpeed * Time.deltaTime;
            }
            else if (Input.DownInput)
            {
                Property.CurrentTongueLength += Property.tongueLengthChangeSpeed * Time.deltaTime;
            }
        }
    }
}