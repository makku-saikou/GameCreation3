// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_16
// Description:
// -------------------------------------------------

using Common.FSM;
using PurpleFlowerCore;

namespace GamePlay.Player.PlayerState
{
    public class AirState : PlayerStateBase
    {
        
        public AirState(PlayerController player, string name) : base(player, name)
        {
        }

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

        // public override void UpdateCallback(float deltaTime)
        // {
        //     base.UpdateCallback(deltaTime);
        //     CheckInput();
        //     CheckMovementState();
        // }


    }
}