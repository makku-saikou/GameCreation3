// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_3_20
// Description:
// -------------------------------------------------

using Common.FSM;
using PurpleFlowerCore.Utility;
using UnityEngine;

namespace GamePlay.Player.PlayerState
{
    public class OnPillarState : PlayerStateBase
    {
        public OnPillarState(PlayerController player, string name) : base(player, name) { }
        
        public override void EnterCallback(HState prev)
        {
            base.EnterCallback(prev);
            Player.AddGravityEffect("OnPillar", 0);
            Rb.velocity = Vector2.zero;
            Player.Head.SetShow(false);
            Player.transform.position = new Vector3(Property.MaxClimbHeight.x,
                Player.transform.position.y, Player.transform.position.z);
            Player.ResetTransform();
        }
        
        public override void ExitCallback(HState next)
        {
            base.ExitCallback(next);
            Player.RemoveGravityEffect("OnPillar");
            Player.Head.SetShow(true);
            ClimbJump();
            
            Property.ClimbFlag = false;
            DelayUtility.Delay(Config.climbCD, () =>
            {
                Property.ClimbFlag = true;
            });
        }

        public override void UpdateCallback(float deltaTime)
        {
            base.UpdateCallback(deltaTime);
            UpdateAni();
        }

        public override void FixedUpdateCallback()
        {
            base.FixedUpdateCallback();
            if(Input.UpInput && Player.transform.position.y < Property.MaxClimbHeight.y)
            {
                Rb.velocity = new Vector2(0, Config.climbPileSpeed);
            }
            else if(Input.DownInput)
            {
                Rb.velocity = new Vector2(0, -Config.climbPileSpeed);
            }
            else
            {
                Rb.velocity = new Vector2(Rb.velocity.x, 0);
            }
        }

        private void UpdateAni()
        {
            if(Input.UpInput || Input.DownInput)
            {
                Property.AniMove = true;
            }
            else
            {
                Property.AniMove = false;
            }
        }

        private void ClimbJump()
        {
            if(Input.DownInput) return;
            Vector2 direction = Config.climbJumpDirection;
            if(Input.MovementInput == 0)
                direction = Vector2.up;
            else if (Input.MovementInput < 0)
                direction = new Vector2(-direction.x, direction.y);
            
            Rb.AddForce(direction * Config.climbJumpForce, ForceMode2D.Impulse);
        }
    }
}