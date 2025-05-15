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
            Tongue.OnTongueLaunch += OnLaunch;
            Tongue.OnTongueIdle += OnRetracted;
        }

        public override void ExitCallback(HState next)
        {
            base.ExitCallback(next);
            PFCLog.Debug("Exit Air State");
            Player.Head.SetShow(true);
            Tongue.OnTongueLaunch -= OnLaunch;
            Tongue.OnTongueIdle -= OnRetracted;
            OnRetracted();
        }
        
        public override void UpdateCallback(float deltaTime)
        {
            base.UpdateCallback(deltaTime);
            if(Input.JumpInputDown)
                Property.ResetPreJumpBufferFlag();
        }

        private void OnLaunch()
        {
            var v = Rb.velocity;
            v *= Config.launchDragScale;
            Rb.velocity = v;
            Rb.gravityScale = 0;
            
            var headRight = Player.Head.transform.right;
            Entity.right = Player.Head.transform.right;
            Player.Head.transform.right = headRight;
            Player.Head.SetShow(true);
            Property.IsAirLaunching = true;
        }
        
        private void OnRetracted()
        {
            Rb.gravityScale = Config.gravityScale;
            
            Player.ResetTransform();
            Player.Head.SetShow(false);
            Property.IsAirLaunching = false;
        }
        
        public override void FixedUpdateCallback()
        {
            base.FixedUpdateCallback();
            
            if (Input.MovementInput != 0 && !Property.IsLaunching)
            {
                Rb.AddForce(new Vector2(Config.xForceInAir * Input.MovementInput, 0), ForceMode2D.Force);
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
            if (!Input.JumpInput || velocity.y < 0 || !Property.JumpBufferFlag)
            {
                velocity = new Vector2(velocity.x, velocity.y - Config.variableJumpForce);
            }
            Rb.velocity = velocity;
        }
    }
}