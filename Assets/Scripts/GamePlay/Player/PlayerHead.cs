// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_08
// File: PlayerHead.cs
// Description: 头部的控制逻辑,调用舌头的相关方法
// -------------------------------------------------

using System.Collections.Generic;
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
        [SerializeField] private List<RuntimeAnimatorController> controllers;
        [SerializeField] private PlayerController player;
        public PlayerController Player => player;
        private PlayerProperty Property => player.Property;
        private PlayerConfig Config => player.Config;
        private PlayerInputBase PlayerInput => player.Input;
        public DirectionLimit DirectionLimit;
        private float _currentMouthOpen;
        private bool _targetMouthOpen;
        private float _currentLaunchCD;
        private float _currentBlinkCD;

        private void Start()
        {
            _currentBlinkCD = Config.blinkFrequency;
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
            RetractTongue();
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
            _currentBlinkCD -= Time.deltaTime;
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
            if(_currentMouthOpen >= 0.01f)
                animator.Play("Head_Close", 0, _currentMouthOpen);
            else
            {
                if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Head_Idle"))
                    animator.Play("Head_Idle", 0, 1);
                if(_currentBlinkCD <= 0)
                {
                    _currentBlinkCD = Config.blinkFrequency;
                    animator.Play("Head_Idle", 0, 0);
                }
            }
        }

        public void ChangeColor(EPlayerColor from, EPlayerColor to)
        {
            animator.runtimeAnimatorController = controllers[(int)to];
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