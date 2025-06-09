// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_20
// Description:
// -------------------------------------------------

using Common.FSM;
using PurpleFlowerCore;
using UnityEngine;
using System.Collections;
using GamePlay.Player.Particle;

namespace GamePlay.Player.PlayerState
{
    public class HangState : PlayerStateBase
    {
        public HangState(PlayerController player, EPlayerState name) : base(player, name)
        { }

        public override void EnterCallback(HState prev)
        {
            base.EnterCallback(prev);
            PFCLog.Debug("Enter Hang State");
            Player.AddGravityEffect("Hang", Config.hangGravityScale);
            Player.Head.SetShow(false);
        }

        public override void ExitCallback(HState next)
        {
            base.ExitCallback(next);
            PFCLog.Debug("Exit Hang State");
            Player.RemoveGravityEffect("Hang");
            Player.Rb.drag = 0;
            Player.Head.SetShow(true);
            Particle.Stop<HangTail>();

            // 补偿力
            var direction = Player.Entity.right;
            if (Property.CurrentHongAngle < 0) direction = -direction;
            float compensating = Config.hangForceCompensate * Mathf.Abs(Property.CurrentHongAngle) / 90;
            Rb.AddForce(direction * compensating, ForceMode2D.Impulse);
            PFCLog.Debug("HangState", $"Compensating Force: {compensating} Direction: {direction}");
            Particle.Get<JumpJet>().Play(Input.MovementInput);
        }

        public override void UpdateCallback(float deltaTime)
        {
            base.UpdateCallback(deltaTime);

            if (Input.JumpInputDown)
            {
                Player.Head.RetractTongue();
                HangJump();
            }

            Tail();
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
            Player.Entity.up = direction;
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

        private void Tail()
        {
            PFCLog.Debug("HangState", $"Tail Speed: {Rb.velocity.magnitude}");
            if (Rb.velocity.sqrMagnitude < Config.hangTrailSpeedThreshold * Config.hangTrailSpeedThreshold)
            {
                Particle.Stop<HangTail>();
            }
            else
            {
                Particle.Play<HangTail>();
            }
        }
    }
}