// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_04_19
// Description:
// -------------------------------------------------

using UnityEngine;
using Common.FSM;
using PurpleFlowerCore.Utility;
using Unity.VisualScripting;

namespace GamePlay.Player.PlayerState
{
    public class SwimState : PlayerStateBase
    {
        public SwimState(PlayerController player, EPlayerState name) : base(player, name) { }
        private float _currentDashCD;
        private bool _canSwim = true;
        
        public override void EnterCallback(HState prev)
        {
            base.EnterCallback(prev);
            Player.Head.SetShow(false);
            Player.AddGravityEffect("OnBackground", 0);
            Rb.velocity = Vector2.zero;
            Property.HeadCanLaunch = false;
            Rb.drag = Config.swimDrag;
            _currentDashCD = 0;
        }
        
        public override void ExitCallback(HState next)
        {
            base.ExitCallback(next);
            Player.Head.SetShow(true);
            Player.RemoveGravityEffect("OnBackground");
            Property.HeadCanLaunch = true;
            Rb.drag = 0;
        }

        public override void UpdateCallback(float deltaTime)
        {
            base.UpdateCallback(deltaTime);
            if(Input.JumpInputDown && _currentDashCD <= 0)
                Dash();
            _currentDashCD -= deltaTime;
        }

        public override void FixedUpdateCallback()
        {
            base.FixedUpdateCallback();
            if(_canSwim)
                Swim();
        }
        
        public override void LateUpdateCallback(float deltaTime)
        {
            base.LateUpdateCallback(deltaTime);
            UpdateAni();
        }
        
        private void UpdateAni()
        {
            if (Rb.velocity.sqrMagnitude >= 4 || Input.DirectionInput != Vector2.zero)
            {
                Property.AniMove = true;
            }
            else
            {
                Property.AniMove = false;
            }
        }

        private void Swim()
        {
            Player.Entity.up = Input.DirectionInput == Vector2.zero ? Player.Entity.up : Input.DirectionInput;
            var moveDirection = Input.DirectionInput;
            if (moveDirection != Vector2.zero)
            {
                Rb.AddForce(moveDirection * Config.swimSpeed, ForceMode2D.Force);
            }
        }

        private void Dash()
        {
            _canSwim = false;
            DelayUtility.Delay(Config.swimDashRecoverTime, () =>
            {
                _canSwim = true;
            });
            Player.Entity.up = Input.AttentionDirection;
            Rb.AddForce(Input.AttentionDirection * Config.swimDashForce, ForceMode2D.Impulse);
            _currentDashCD = Config.swimDashCD;
        }
    }
}