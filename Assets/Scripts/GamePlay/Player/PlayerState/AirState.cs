// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_16
// Description:
// -------------------------------------------------

using Common.FSM;
using PurpleFlowerCore;
using PurpleFlowerCore.Resource;
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
            if (_p.MovementInput != 0)
            {
                _rb.AddForce(new Vector2(_p.xForceInAir * _p.MovementInput, 0), ForceMode2D.Force);
            }
            var velocity = _rb.velocity;
            // todo: 最大限制优化
            if (Mathf.Abs(velocity.x) > _p.XMaxSpeed)
            {
                velocity = new Vector2(Mathf.Sign(velocity.x) * _p.XMaxSpeed, velocity.y);
                _rb.velocity = velocity;
            }
            if (Mathf.Abs(velocity.y) > _p.YMaxSpeed)
            {
                velocity = new Vector2(velocity.x, Mathf.Sign(velocity.y) * _p.YMaxSpeed);
                _rb.velocity = velocity;
            }
        }
    }
}