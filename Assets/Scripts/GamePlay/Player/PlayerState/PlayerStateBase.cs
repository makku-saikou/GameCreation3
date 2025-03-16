// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_15
// File: OnGround.cs
// Description:
// -------------------------------------------------

using System;
using Common.FSM;
using UnityEngine;

namespace GamePlay.Player.PlayerState
{
    // 我们暂时将所有的逻辑都放在这个类里，之后会考虑拆分
    public class PlayerStateBase : HState
    {
        private PlayerProperty _p;
        private PlayerController _player;
        private Rigidbody2D _rb;

        public PlayerStateBase(PlayerController player)
        {
            _player = player;
            _p = player.Property;
            _rb = player.Rb;
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
            CheckSurroundings();
        }

        private void CheckInput()
        {
            _p.movementInput = Input.GetAxisRaw("Horizontal");

            if (Input.GetButtonDown("Jump"))
            {
                if (_p.isGrounded || (_p.amountOfJumpLeft > 0 && !_p.isTouchingWall))
                    NormalJump();
                else
                    _p.jumpTimer = _p.jumpTimerSet;
            }

            if (_p.checkVariableJump && !Input.GetButton("Jump"))
            {
                _p.checkVariableJump = false;
                var velocity = _rb.velocity;
                velocity = new Vector2(velocity.x, velocity.y * _p.variableJumpHeightMultiplier);
                _rb.velocity = velocity;
            }
        }

        private void CheckMovementState()
        {
            _p.isWalking = Math.Abs(_rb.velocity.x) > 0.01f; // rigidbody的速度在移动时会有一个极小的值，故为>0.01，其他小值也可，令人费解的bug
        }
        
        private void CheckJumpState()
        {
            if (_p.isGrounded && _rb.velocity.y <= 0.01f) // 着陆时
            {
                _p.amountOfJumpLeft = _p.amountOfJump;
                _p.checkVariableJump = false;
            }

            if (_p.isTouchingWall) _p.checkVariableJump = false;

            _p.canNormalJump = _p.amountOfJumpLeft > 0;
            
            if (_p.jumpTimer > 0)
            {
                if (_p.isGrounded) 
                    NormalJump();
                
                _p.jumpTimer -= Time.deltaTime;
            }
        }
        
        private void CheckSurroundings()
        {
            _p.isGrounded = 
                Physics2D.OverlapCircle(_p.groundCheckPoint.position, _p.groundCheckRadius, _p.groundLayer);
        }
        
        private void ApplyMovement()
        {
            if (!_p.isGrounded && !_p.isWallSliding && _p.movementInput == 0)
            {
                // 当在空中且没有输入时，会受到空气阻力
                var velocity = _rb.velocity;
                velocity = new Vector2(velocity.x * _p.airDragMultiplier, velocity.y);
                _rb.velocity = velocity;
            }
            else if (_p.canMove)
            {
                // 正常移动
                if(_p.movementInput != 0)
                    _rb.velocity = new Vector2(_p.movementSpeed * _p.movementInput, _rb.velocity.y);
            }
        }
        
        private void NormalJump()
        {
            if (!_p.canNormalJump) return;
            
            _rb.velocity = new Vector2(_rb.velocity.x, _p.jumpForce);
                
            _p.amountOfJumpLeft--;
            _p.jumpTimer = 0;
            _p.checkVariableJump = true;
        }
    }
}