// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_16
// Description:
// -------------------------------------------------

using Common.FSM;
using Common.Manager;
using GamePlay.Player.Particle;
using PurpleFlowerCore;
using UnityEngine;

namespace GamePlay.Player.PlayerState
{
    public class OnGroundState : PlayerStateBase
    {
        public OnGroundState(PlayerController player, EPlayerState name) : base(player, name) { }
        private float _soundTimer = 0f;
        public override void EnterCallback(HState prev)
        {
            base.EnterCallback(prev);

            PFCLog.Debug("Enter OnGround State");
            Player.ResetTransform();
            Player.Head.SetShow(true);
            Player.Head.Tongue.DoRetract();
        }
        
        public override void ExitCallback(HState next)
        {
            base.ExitCallback(next);
            PFCLog.Debug("Exit OnGround State");
        }
        
        public override void UpdateCallback(float deltaTime)
        {
            base.UpdateCallback(deltaTime);
            CheckInput();
            CheckJumpState();
            _soundTimer -= deltaTime;
        }

        public override void LateUpdateCallback(float deltaTime)
        {
            base.LateUpdateCallback(deltaTime);
            CheckAni();
        }

        public override void FixedUpdateCallback()
        {
            base.FixedUpdateCallback();
            ApplyMovement();
        }

        private void CheckInput()
        {
            if (Input.JumpInputDown)
            {
                if (Property.IsGrounded || (Property.AmountOfJumpLeft > 0))
                    NormalJump();
            }
        }

        private void CheckAni()
        {
            // rigidbody的速度在移动时会有一个极小的值，故为>0.01，其他小值也可，令人费解的bug
            // Property.IsWalking = Math.Abs(Rb.velocity.x) > 0.01f; 
            Property.AniMove = Mathf.Abs(Rb.velocity.x) > 0.01f;
        }
        
        private void CheckJumpState()
        {
            // todo: 整理逻辑
            if (Property.IsGrounded && Rb.velocity.y <= 0.01f) // 着陆时
            {
                Property.AmountOfJumpLeft = Config.amountOfJump;
            }
            Property.CanJump = Property.AmountOfJumpLeft > 0;
            if (Property.PreJumpBufferFlag)
            {
                Property.PreJumpBufferFlag = false;
                NormalJump();
            }
        }
        
        private void ApplyMovement()
        {
            if (!Property.CanMove) return;
            float xInputExtent = Mathf.Abs(Input.XInputExtent);
            if(xInputExtent < Config.onGroundWalkToRunCoefficient)
            {
                var velocity = Mathf.Lerp(Config.onGroundWalkSpeed, Config.onGroundRunSpeed, xInputExtent / Config.onGroundWalkToRunCoefficient);
                Rb.velocity = new Vector2(velocity * Input.MovementInput, Rb.velocity.y);
            }
            else
            {
                Particle.Play<GroundTail>();
                Rb.velocity = new Vector2(Config.onGroundRunSpeed * Input.MovementInput, Rb.velocity.y);
            }

            if (xInputExtent > 0) // 有输入
            {
                if (_soundTimer <= 0)
                {
                    _soundTimer = Config.footstepInterval;
                    int index = Random.Range(1, 7);
                    string path = "脚步声/脚步声-" + index;
                    AudioManager.PlayEffect(path, Player.Entity);
                }
            }
        }
        
        private void NormalJump()
        {
            if (!Property.CanJump) return;
            Rb.velocity = new Vector2(Rb.velocity.x, Config.jumpForce);
            Property.AmountOfJumpLeft--;
            Property.ResetWallJumpTimer();
            Property.ResetJumpBufferFlag();
            
            Particle.Get<JumpJet>().Play(Input.MovementInput);
            AudioManager.PlayEffect("玩家跳跃音效",Entity);
        }
    }
}