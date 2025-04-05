// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_16
// Description:
// -------------------------------------------------

using System;
using Common.FSM;
using PurpleFlowerCore;
using UnityEngine;

namespace GamePlay.Player.PlayerState
{
    // todo: 由于状态机的引入,需要删除原来无用逻辑
    public class OnGroundState : PlayerStateBase
    {
        private float _jumpTimer; // 跳跃计时器，提供输入提前量，优化下一次跳跃的手感
        
        public OnGroundState(PlayerController player, string name) : base(player, name) { }
        
        public override void EnterCallback(HState prev)
        {
            base.EnterCallback(prev);
            PFCLog.Debug("Enter OnGround State");
            
        }
        
        public override void UpdateCallback(float deltaTime)
        {
            base.UpdateCallback(deltaTime);
            CheckInput();
            CheckMovementState();
            CheckJumpState();
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
                if (P.IsGrounded || (P.AmountOfJumpLeft > 0))
                    NormalJump();
                else
                    _jumpTimer = P.jumpTimerSet;
            }
        }

        private void CheckMovementState()
        {
            P.IsWalking = Math.Abs(Rb.velocity.x) > 0.01f; // rigidbody的速度在移动时会有一个极小的值，故为>0.01，其他小值也可，令人费解的bug
        }
        
        private void CheckJumpState()
        {
            // todo: 整理逻辑
            if (P.IsGrounded && Rb.velocity.y <= 0.01f) // 着陆时
            {
                P.AmountOfJumpLeft = P.amountOfJump;
            }

            P.CanJump = P.AmountOfJumpLeft > 0;
            
            if (_jumpTimer > 0)
            {
                if (P.IsGrounded) 
                    NormalJump();
                
                _jumpTimer -= Time.deltaTime;
            }
        }
        
        private void ApplyMovement()
        {
            if (!P.CanMove) return;
            Rb.velocity = new Vector2(P.onGroundSpeed * Input.MovementInput, Rb.velocity.y);
        }
        
        private void NormalJump()
        {
            if (!P.CanJump) return;
            
            Rb.velocity = new Vector2(Rb.velocity.x, P.jumpForce);
            
            P.AmountOfJumpLeft--;
            _jumpTimer = 0;
        }
    }
}