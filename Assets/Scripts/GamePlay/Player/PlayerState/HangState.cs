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
            Rb.gravityScale = Config.hangGravityScale;
            // todo: 解耦
            Player.Head.SetShow(false);
        }
        
        public override void ExitCallback(HState next)
        {
            base.ExitCallback(next);
            PFCLog.Debug("Exit Hang State");
            Rb.gravityScale = Config.gravityScale;
            Player.Rb.drag = 0;
            Property.XMaxSpeed = Mathf.Max(Mathf.Abs(Rb.velocity.x), Property.XMaxSpeed);
            Property.YMaxSpeed = Mathf.Max(Mathf.Abs(Rb.velocity.y), Property.YMaxSpeed);
            Player.Head.SetShow(true);
            
            Player.Entity.transform.right = Property.FacingDirection * Vector2.right;
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
            Rb.velocity += new Vector2(0, Config.hangJumpForce);
        }

        private void BodyDirection()
        {
            var direction = Property.HangPoint - Player.transform.position;
            direction.Normalize();
            Player.Entity.transform.up = direction;
        }

        private void Move()
        {
            Player.Rb.drag = Input.MovementInput == 0 ? Config.hangDrag : 0;
            if (Input.MovementInput != 0)
            {
                Player.Rb.AddForce(new Vector2(Config.hangSwayForce * Input.MovementInput, 0), ForceMode2D.Force);
            }
        }
        
        private void ChangeTongueLength()
        {
            if (Input.UpInput)
            {
                Property.CurrentTongueLength -= Config.tongueLengthChangeSpeed * Time.deltaTime;
            }
            else if (Input.DownInput)
            {
                Property.CurrentTongueLength += Config.tongueLengthChangeSpeed * Time.deltaTime;
            }
        }
    }
}