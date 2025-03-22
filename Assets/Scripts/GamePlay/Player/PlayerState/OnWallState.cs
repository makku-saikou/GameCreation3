// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_3_20
// Description:
// -------------------------------------------------

using Common.FSM;
using PurpleFlowerCore;
using PurpleFlowerCore.Resource;
using PurpleFlowerCore.Utility;
using UnityEngine;

namespace GamePlay.Player.PlayerState
{
    public class OnWallState : PlayerStateBase
    {
        public OnWallState(PlayerController player, string name) : base(player, name) { }
        
        public override void EnterCallback(HState prev)
        {
            base.EnterCallback(prev);
            _rb.gravityScale = 0;
            AddressableModule addressableModule = new AddressableModule();
            addressableModule.Load<Sprite>("Body0", sprite =>
            {
                _player.SpriteRenderer.sprite = sprite.Result;
            });
        }

        public override void ExitCallback(HState next)
        {
            base.ExitCallback(next);
            _rb.gravityScale = _p.gravityScale;
        }

        public override void UpdateCallback(float deltaTime)
        {
            base.UpdateCallback(deltaTime);
            if (_p.JumpInput)
            {
                PFCLog.Debug("Wall Jump");
                Vector2 direction = _p.WallJumpDirection;
                if (_p.IsRightWall)
                {
                    direction.x = -direction.x;
                }
                _rb.AddForce(direction * _p.wallJumpForce, ForceMode2D.Impulse);
                _p.WallJumpFlag = true;
                DelayUtility.Delay(_p.wallJumpTimerSet, () =>
                {
                    _p.WallJumpFlag = false;
                });
            }
        }

        public override void FixedUpdateCallback()
        {
            base.FixedUpdateCallback();
            if(!_p.WallJumpFlag)
                _rb.velocity = new Vector2(0, -_p.wallSlideSpeed);
        }
    }
}