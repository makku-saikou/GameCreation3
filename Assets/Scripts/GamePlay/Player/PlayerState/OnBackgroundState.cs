// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_04_19
// Description:
// -------------------------------------------------

using UnityEngine;
using Common.FSM;
using PurpleFlowerCore;

namespace GamePlay.Player.PlayerState
{
    public class OnBackgroundState : PlayerStateBase
    {
        public OnBackgroundState(PlayerController player, string name) : base(player, name) { }
        private float _currentDashCD;
        
        public override void EnterCallback(HState prev)
        {
            base.EnterCallback(prev);
            PFCLog.Debug("Enter OnBackground State");
            Player.Head.SetShow(false);
            Rb.gravityScale = 0;
            Rb.velocity = Vector2.zero;
            Property.IsOnColorBlock = true;
            Property.HeadCanLaunch = false;
            Rb.drag = Config.swimDrag;
            _currentDashCD = 0;
        }
        
        public override void ExitCallback(HState next)
        {
            base.ExitCallback(next);
            PFCLog.Debug("Exit OnBackground State");
            Player.Head.SetShow(true);
            Rb.gravityScale = Config.gravityScale;
            Property.IsOnColorBlock = false;
            Property.HeadCanLaunch = true;
            Rb.drag = 0;
            Property.XMaxSpeed = Mathf.Max(Mathf.Abs(Rb.velocity.x), Property.XMaxSpeed);
            Property.YMaxSpeed = Mathf.Max(Mathf.Abs(Rb.velocity.y), Property.YMaxSpeed);
        }

        public override void UpdateCallback(float deltaTime)
        {
            base.UpdateCallback(deltaTime);
            var bodyDirection = Input.AttentionDirection;
            Player.Entity.up = bodyDirection;
            if(Input.JumpInputDown && _currentDashCD <= 0)
                Dash();
            _currentDashCD -= deltaTime;
        }

        public override void FixedUpdateCallback()
        {
            base.FixedUpdateCallback();
            var moveDirection = Input.DirectionInput;
            if (moveDirection != Vector2.zero)
            {
                Rb.AddForce(moveDirection * Config.swimSpeed, ForceMode2D.Force);
            }
        }

        private void Dash()
        {
            Rb.AddForce(Input.AttentionDirection * Config.swimDashForce, ForceMode2D.Impulse);
            _currentDashCD = Config.swimDashCD;
        }
    }
}