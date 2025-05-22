// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_08
// File: PlayerHead.cs
// Description: 头部的控制逻辑,调用舌头的相关方法
// -------------------------------------------------

using GamePlay.Player.PlayerInput;
using PurpleFlowerCore;
using UnityEngine;

namespace GamePlay.Player
{
    public delegate Vector3 DirectionLimit(Vector3 direction);
    
    public class PlayerHead : MonoBehaviour
    {
        [SerializeField] private PlayerTongue tongue;
        public PlayerTongue Tongue => tongue;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Animator animator;
        [SerializeField] private PlayerController player;
        public PlayerController Player => player;
        private PlayerProperty Property => player.Property;
        private PlayerConfig Config => player.Config;
        private PlayerInputBase PlayerInput => player.Input;
        public DirectionLimit DirectionLimit;
        private float _currentMouthOpen;
        private float _targetMouthOpen;

        private void Start()
        {
            player.StateMachine.OnStateChanged += (from, to) =>
            {
                if(tongue.TongueState == ETongueState.Launching && to.Name != "Hang")
                    RetractTongue();
            };
            
            tongue.OnTongueLaunch += () =>
            {
                _targetMouthOpen = 1;
                PFCLog.Debug("Head","Tongue Launch");
            };
            
            tongue.OnTongueRetract += () =>
            {
                _targetMouthOpen = 0;
                PFCLog.Debug("Head","Tongue Retract");
            };
        }

        private void Update()
        {
            UpdateDirection();

            if (PlayerInput.LaunchDown)
            {
                LaunchTongue();
            }
            if (PlayerInput.LaunchUp)
            {
                RetractTongue();
            }
            if (PlayerInput.ConnectInteractDown)
            {
                InteractTongue();
            }
        }

        private void FixedUpdate()
        {
            UpdateAni();
        }

        private void UpdateDirection()
        {
            if (!Property.HeadCanMove) return;
            var direction = PlayerInput.AttentionDirection;
            if (DirectionLimit != null)
                direction = DirectionLimit(direction); // 确保此处输入的方向是归一化的
            transform.right = direction;
        }

        private void UpdateAni()
        {
            if(Mathf.Approximately(_targetMouthOpen, 1))
                _currentMouthOpen = Mathf.MoveTowards(_currentMouthOpen, _targetMouthOpen, Config.openMouthSpeed);
            else if(Mathf.Approximately(_targetMouthOpen, 0))
                _currentMouthOpen = Mathf.MoveTowards(_currentMouthOpen, _targetMouthOpen, Config.closeMouthSpeed);
            animator.Play("Head_Close", 0, _currentMouthOpen);
        }

        #region Tongue

        private void LaunchTongue()
        {
            if (!Property.HeadCanLaunch) return;
            tongue.Launch(transform.right);
        }

        public void RetractTongue()
        {
            tongue.Retract();
        }

        private void InteractTongue()
        {
            tongue.Interact();
        }

        #endregion

        public void SetShow(bool show)
        {
            spriteRenderer.enabled = show;
        }
    }
}