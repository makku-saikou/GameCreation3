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
            _rb.gravityScale = _p.hangGravityScale;
        }

        public override void UpdateCallback(float deltaTime)
        {
            base.UpdateCallback(deltaTime);
            
            if(_p.JumpInput)
            {
                // todo: 解耦
                _player.Head.RetractTongue();
                HangJump();
            }
        }

        public override void FixedUpdateCallback()
        {
            base.FixedUpdateCallback();
                
            _player.Rb.drag = _p.MovementInput == 0 ? _p.hangDrag : 0;
            if (_p.MovementInput != 0)
            {
                _player.Rb.AddForce(new Vector2(_p.hangSwayForce * _p.MovementInput, 0), ForceMode2D.Force);
            }
        }

        public override void ExitCallback(HState next)
        {
            base.ExitCallback(next);
            _rb.gravityScale = _p.gravityScale;
            _player.Rb.drag = 0;
            _p.XMaxSpeed = Mathf.Abs(_p.XMaxSpeed);
        }

        private void HangJump()
        {
            _rb.velocity += new Vector2(0, _p.jumpForce);
        }
    }
}