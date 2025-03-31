// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_3_20
// Description:
// -------------------------------------------------

using Common.FSM;
using UnityEngine;

namespace GamePlay.Player.PlayerState
{
    public class OnPillarState : PlayerStateBase
    {
        public OnPillarState(PlayerController player, string name) : base(player, name) { }
        
        public override void EnterCallback(HState prev)
        {
            base.EnterCallback(prev);
            _rb.gravityScale = 0;
            _rb.velocity = Vector2.zero;
        }

        public override void FixedUpdateCallback()
        {
            base.FixedUpdateCallback();
            if(_p.UpInput && _player.transform.position.y < _p.maxClimbHeight)
            {
                _rb.velocity = new Vector2(0, _p.climbSpeed);
            }
            else if(_p.DownInput)
            {
                _rb.velocity = new Vector2(0, -_p.climbSpeed);
            }
            else
            {
                _rb.velocity = new Vector2(_rb.velocity.x, 0);
            }
        }

        public override void ExitCallback(HState next)
        {
            base.ExitCallback(next);
            _rb.gravityScale = _p.gravityScale;
        }
    }
}