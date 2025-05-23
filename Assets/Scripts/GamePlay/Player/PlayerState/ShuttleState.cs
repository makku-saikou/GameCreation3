// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_05_24
// Description:
// -------------------------------------------------

using Common.FSM;
using UnityEngine;

namespace GamePlay.Player.PlayerState
{
    public class ShuttleState : PlayerStateBase
    {
        public ShuttleState(PlayerController player, string name) : base(player, name)
        {
        }
        
        public override void EnterCallback(HState prev)
        {
            base.EnterCallback(prev);
            Player.Head.SetShow(false);
            Rb.gravityScale = 0;
            var direction = Rb.velocity.normalized;
            direction.Normalize();
            Player.Entity.up = direction;
            Rb.velocity = direction * Config.shuttleSpeed;
        }
        
        public override void ExitCallback(HState next)
        {
            base.ExitCallback(next);
            Player.Head.SetShow(true);
            Rb.gravityScale = Config.gravityScale;
            Player.ResetTransform();
            Property.XMaxSpeed = Mathf.Max(Mathf.Abs(Rb.velocity.x), Property.XMaxSpeed);
            Property.YMaxSpeed = Mathf.Max(Mathf.Abs(Rb.velocity.y), Property.YMaxSpeed);
        }
    }
}