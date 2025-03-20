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
            AddressableModule addressableModule = new AddressableModule();
            addressableModule.Load<Sprite>("Body1", sprite =>
            {
                _player.SpriteRenderer.sprite = sprite.Result;
            });
        }

        public override void ExitCallback(HState next)
        {
            base.ExitCallback(next);
            PFCLog.Debug("Exit Air State");
        }

        public override void UpdateCallback(float deltaTime)
        {
            base.UpdateCallback(deltaTime);
            _p.MovementInput = Input.GetAxisRaw("Horizontal");
        }
        
        public override void FixedUpdateCallback()
        {
            base.FixedUpdateCallback();
            if (_p.MovementInput != 0)
            {
                _rb.AddForce(new Vector2(10 * _p.MovementInput, 0), ForceMode2D.Force);
                var velocity = _rb.velocity;
                if (Mathf.Abs(velocity.x) > _p.xMixSpeedInAir)
                {
                    velocity = new Vector2(Mathf.Sign(velocity.x) * _p.xMixSpeedInAir, velocity.y);
                    _rb.velocity = velocity;
                }
            }
        }
    }
}