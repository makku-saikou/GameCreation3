// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_3_24
// Description:
// -------------------------------------------------

using Common.FSM;
using GamePlay.Item.Platform;
using PurpleFlowerCore.Utility;
using UnityEngine;

namespace GamePlay.Player.PlayerState
{
    public class SmashState : PlayerStateBase
    {
        public SmashState(PlayerController player, EPlayerState name) : base(player, name) {}
        
        public override void EnterCallback(HState prev)
        {
            base.EnterCallback(prev);
            Property.HasSmashLanded = false;
            Player.Head.SetShow(false);
            Property.HeadCanLaunch = false;
            Rb.velocity = new Vector2(0, Rb.velocity.y);
            Player.AddGravityEffect("Smash", Config.smashGravityScale, Config.smashGravityScaleTime);
            DelayUtility.Delay(Config.smashGravityScaleTime, () =>
            {
                Rb.velocity = new Vector2(0, -Config.smashVelocity);
                Animator.Play("Smash_Down");
            });
        }

        public override void LateUpdateCallback(float deltaTime)
        {
            base.LateUpdateCallback(deltaTime);
            bool trampoline = Property.CurrentGroundCollider 
                              && Property.CurrentGroundCollider.gameObject.CompareTag("Trampoline");
            if (Property.IsGrounded || trampoline)
            {
                float bounceForce;
                if (trampoline)
                {
                    bounceForce = Property.CurrentGroundCollider.GetComponentInParent<Trampoline>().BounceForce;
                    // bounceForce = 0;
                }
                else
                {
                    bounceForce = Config.smashBounceForce;
                }
                Bounce(bounceForce);
                Player.CameraShakeFeedback.PlayFeedbacks();
                DelayUtility.Delay(0.1f, () =>
                {
                    Property.HasSmashLanded = true;
                });
            }
        }

        public override void ExitCallback(HState next)
        {
            base.ExitCallback(next);
            Property.HasSmashLanded = false;
            Player.Head.SetShow(true);
            Property.HeadCanLaunch = true;
            Property.SmashFlag = false;
            DelayUtility.Delay(Config.smashCD, () =>
            {
                Property.SmashFlag = true;
            });
        }
        
        private void Bounce(float bounceForce)
        {
            if(Mathf.Abs(Input.MovementInput) > 0.9f)
            {
                var direction = Config.smashDirection;
                direction = new Vector3(direction.x * Mathf.Sign(Input.MovementInput), direction.y, 0);
                direction.Normalize();
                Rb.velocity = direction * bounceForce;
            }
            else
                Rb.velocity = new Vector2(Rb.velocity.x, bounceForce);
        }
    }
}
