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
            _p.movementInput = Input.GetAxisRaw("Horizontal");
        }
        
        public override void FixedUpdateCallback()
        {
            base.FixedUpdateCallback();
            // if (!_p.isWallSliding && _p.movementInput != 0)
            // {
            // var velocity = _rb.velocity;
            // velocity = new Vector2(_p.movementSpeed * _p.movementInput, velocity.y);
            // _rb.velocity = velocity;
            if (_p.movementInput != 0)
            {
                _rb.AddForce(new Vector2(10 * _p.movementInput, 0), ForceMode2D.Force);
                var velocity = _rb.velocity;
                if (Mathf.Abs(velocity.x) > 30)
                {
                    velocity = new Vector2(10 * _p.movementInput, velocity.y);
                    _rb.velocity = velocity;
                }
            }
            // }
            // else if (_p.isWallSliding && _p.movementInput == 0)
            // {
            //     var velocity = _rb.velocity;
            //     velocity = new Vector2(velocity.x * _p.fallMultiplier, velocity.y);
            //     _rb.velocity = velocity;
            // }
        }
    }
}