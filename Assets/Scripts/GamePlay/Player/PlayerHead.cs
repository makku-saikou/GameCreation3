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
        // [SerializeField] private SpriteRenderer headBackground;
        public PlayerController Player => player;
        private PlayerProperty Property => player.Property;
        private PlayerConfig Config => player.Config;
        private PlayerInputBase PlayerInput => player.Input;
        public DirectionLimit DirectionLimit;
        private float _currentMouthOpen;
        private bool _targetMouthOpen;
        private float _currentLaunchCD;

        private void Start()
        {
            player.StateMachine.OnStateChanged += (from, to) =>
            {
                if(tongue.TongueState == ETongueState.Launching && to.Name != "Hang")
                    RetractTongue();
            };
            
            tongue.OnTongueLaunch += () =>
            {
                _targetMouthOpen = true;
                PFCLog.Debug("Head","Tongue Launch");
            };
            
            tongue.OnTongueRetract += () =>
            {
                _targetMouthOpen = false;
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
            _currentLaunchCD -= Time.deltaTime;
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
            if(_targetMouthOpen)
                _currentMouthOpen += Time.deltaTime * Config.openMouthSpeed * Time.deltaTime * 30;
            else
                _currentMouthOpen -= Time.deltaTime * Config.closeMouthSpeed * Time.deltaTime * 30;
            _currentMouthOpen = Mathf.Clamp01(_currentMouthOpen);
            animator.Play("Head_Close", 0, _currentMouthOpen);
            // headBackground.enabled = _currentMouthOpen >= 1;
        }

        #region Tongue

        private void LaunchTongue()
        {
            if (!Property.HeadCanLaunch) return;
            if (_currentLaunchCD > 0) return;
            _currentLaunchCD = Config.launchCD;
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