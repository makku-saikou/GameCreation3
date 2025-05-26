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
        private float _aniSpeedBuffer;
        public AirState(PlayerController player, string name) : base(player, name) { }

        public override void EnterCallback(HState prev)
        {
            base.EnterCallback(prev);
            PFCLog.Debug("Enter Air State");
            Player.Head.SetShow(false);
            Tongue.OnTongueLaunch += OnLaunch;
            Tongue.OnTongueIdle += OnRetracted;
            _aniSpeedBuffer = Animator.speed;
        }

        public override void ExitCallback(HState next)
        {
            base.ExitCallback(next);
            PFCLog.Debug("Exit Air State");
            Player.Head.SetShow(true);
            Tongue.OnTongueLaunch -= OnLaunch;
            Tongue.OnTongueIdle -= OnRetracted;
            OnRetracted();
            Animator.speed = _aniSpeedBuffer;
        }
        
        public override void UpdateCallback(float deltaTime)
        {
            base.UpdateCallback(deltaTime);
            if(Input.JumpInputDown)
                Property.ResetPreJumpBufferFlag();
            UpdateAni();
        }

        private void OnLaunch()
        {
            var v = Rb.velocity;
            v *= Config.launchDragScale;
            Rb.velocity = v;
            Player.AddGravityEffect("AirLaunch", 0);
            
            Entity.up = Player.Head.transform.right;
            Player.Head.transform.right = Entity.up;
            Property.IsAirLaunching = true;
        }
        
        private void OnRetracted()
        {
            Player.RemoveGravityEffect("AirLaunch");
            
            Player.ResetTransform();
            Property.IsAirLaunching = false;
        }
        
        public override void FixedUpdateCallback()
        {
            base.FixedUpdateCallback();
            
            if (Input.MovementInput != 0 && !Property.IsLaunching && Mathf.Abs(Rb.velocity.x) < Config.airMaxSpeed.x)
            {
                Rb.AddForce(new Vector2(Config.xForceInAir * Input.MovementInput, 0), ForceMode2D.Force);
                // Rb.velocity = new Vector2(Config.xForceInAir * Input.MovementInput, Rb.velocity.y);
            }
            var velocity = Rb.velocity;
            
            if (!Input.JumpInput || velocity.y < 0 || !Property.JumpBufferFlag)
            {
                velocity = new Vector2(velocity.x, velocity.y - Config.variableJumpForce);
            }
            Rb.velocity = velocity;

            RecoverMaxSpeed();
        }
        
        private void RecoverMaxSpeed()
        {
            var velocity = Rb.velocity;
            var x = velocity.x;
            var y = velocity.y;
            if (Mathf.Abs(x) > Config.airMaxSpeed.x)
            {
                x = Mathf.Lerp(x, Mathf.Sign(x) * Config.airMaxSpeed.x, Config.airMaxSpeedRecoverScale.x);
            }
            if (Mathf.Abs(y) > Config.airMaxSpeed.y)
            {
                y = Mathf.Lerp(y, Mathf.Sign(y) * Config.airMaxSpeed.y, Config.airMaxSpeedRecoverScale.y);
            }
            velocity = new Vector2(x, y);
            Rb.velocity = velocity;
        }

        private void UpdateAni()
        {
            Animator.speed = Rb.velocity.magnitude * Config.airRotateSpeed * 0.01f;
            Debug.Log(Animator.speed);
        }
    }
}