// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_3_20
// Description:
// -------------------------------------------------

using Common.FSM;
using Common.Manager;
using GamePlay.Player.Particle;
using PurpleFlowerCore.Utility;
using UnityEngine;

namespace GamePlay.Player.PlayerState
{
    public class OnPillarState : PlayerStateBase
    {
        public OnPillarState(PlayerController player, EPlayerState name) : base(player, name) { }
        private float _soundTimer = 0f;
        
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
            if (next.Name != "OnGround")
            {
                Property.ClimbFlag = false;
                DelayUtility.Delay(Config.climbCD, () =>
                {
                    Property.ClimbFlag = true;
                });
            }
        }

        public override void UpdateCallback(float deltaTime)
        {
            base.UpdateCallback(deltaTime);
            if (Input.UpInput || Input.DownInput) // 有输入
            {
                if (_soundTimer <= 0)
                {
                    _soundTimer = Config.footstepInterval;
                    int index = Random.Range(1, 5);
                    string path = "玩家爬杆脚步声/玩家爬杆脚步声-" + index;
                    AudioManager.PlayEffect(path, Player.Entity);
                }
            }
        }

        public override void LateUpdateCallback(float deltaTime)
        {
            base.LateUpdateCallback(deltaTime);
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
            Particle.Get<JumpJet>().Play(Input.MovementInput);
            AudioManager.PlayEffect("玩家跳跃音效",Entity);
        }
    }
}