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
    // 关于在柱子上的实现有待商榷，可以考虑不使用直接控制PlayerController，而是控制假玩家
    public class OnPillarState : PlayerStateBase
    {
        public OnPillarState(PlayerController player, string name) : base(player, name) { }
        
        public override void EnterCallback(HState prev)
        {
            base.EnterCallback(prev);
            _rb.gravityScale = 0;
            // _player.transform.position = 
        }

        public override void ExitCallback(HState next)
        {
            base.ExitCallback(next);
            _rb.gravityScale = _p.gravityScale;
        }
    }
}