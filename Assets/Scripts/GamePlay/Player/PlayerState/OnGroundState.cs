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
    public class OnGroundState : PlayerStateBase
    {
        public OnGroundState(PlayerController player, EPlayerState name) : base(player, name) { }
        
        public override void EnterCallback(HState prev)
        {
            base.EnterCallback(prev);

            PFCLog.Debug("Enter OnGround State");
            Player.ResetTransform();
            Player.Head.SetShow(true);
        }
        
        public override void UpdateCallback(float deltaTime)
        {
            base.UpdateCallback(deltaTime);
            CheckInput();
            CheckJumpState();
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
            if(Mathf.Abs(Input.XInputExtent) < Config.onGroundWalkToRunCoefficient)
                Rb.velocity = new Vector2(Config.onGroundWalkSpeed * Input.MovementInput, Rb.velocity.y);
            else
                Rb.velocity = new Vector2(Config.onGroundRunSpeed * Input.MovementInput, Rb.velocity.y);
        }
        
        private void NormalJump()
        {
            if (!Property.CanJump) return;
            Rb.velocity = new Vector2(Rb.velocity.x, Config.jumpForce);
            Property.AmountOfJumpLeft--;
            Property.ResetWallJumpTimer();
            Property.ResetJumpBufferFlag();
        }
    }
}