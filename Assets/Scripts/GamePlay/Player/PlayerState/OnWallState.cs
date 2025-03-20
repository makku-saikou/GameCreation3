// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_3_20
// Description:
// -------------------------------------------------

using Common.FSM;

namespace GamePlay.Player.PlayerState
{
    public class OnWallState : PlayerStateBase
    {
        public OnWallState(PlayerController player, string name) : base(player, name) { }
        
        public override void EnterCallback(HState prev)
        {
            base.EnterCallback(prev);
            _rb.gravityScale = 0;
        }
    }
}