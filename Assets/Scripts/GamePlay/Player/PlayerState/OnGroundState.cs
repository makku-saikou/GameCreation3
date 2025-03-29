// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_16
// Description:
// -------------------------------------------------

using System;
using Common.FSM;
using PurpleFlowerCore;
using PurpleFlowerCore.Resource;
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
            if (_p.JumpInput)
            {
                if (_p.IsGrounded || (_p.AmountOfJumpLeft > 0))
                    NormalJump();
                else
                    _jumpTimer = _p.jumpTimerSet;
            }

            // if (_p.checkVariableJump && !Input.GetButton("Jump"))
            // {
            //     _p.checkVariableJump = false;
            //     var velocity = _rb.velocity;
            //     velocity = new Vector2(velocity.x, velocity.y * _p.variableJumpHeightMultiplier);
            //     _rb.velocity = velocity;
            // }
        }

        private void CheckMovementState()
        {
            _p.IsWalking = Math.Abs(_rb.velocity.x) > 0.01f; // rigidbody的速度在移动时会有一个极小的值，故为>0.01，其他小值也可，令人费解的bug
        }
        
        private void CheckJumpState()
        {
            // todo: 整理逻辑
            if (_p.IsGrounded && _rb.velocity.y <= 0.01f) // 着陆时
            {
                _p.AmountOfJumpLeft = _p.amountOfJump;
                // _p.checkVariableJump = false;
            }

            // if (_p.isTouchingWall) _p.checkVariableJump = false;

            _p.CanJump = _p.AmountOfJumpLeft > 0;
            
            if (_jumpTimer > 0)
            {
                if (_p.IsGrounded) 
                    NormalJump();
                
                _jumpTimer -= Time.deltaTime;
            }
        }
        
        private void ApplyMovement()
        {
            if (!_p.CanMove) return;
            _rb.velocity = new Vector2(_p.onGroundSpeed * _p.MovementInput, _rb.velocity.y);
            // else
            // {
            //     _rb.velocity = new Vector2(0, _rb.velocity.y);
            // }
        }
        
        private void NormalJump()
        {
            if (!_p.CanJump) return;
            
            _rb.velocity = new Vector2(_rb.velocity.x, _p.jumpForce);
            
            _p.AmountOfJumpLeft--;
            _jumpTimer = 0;
            // _p.checkVariableJump = true;
        }
    }
}