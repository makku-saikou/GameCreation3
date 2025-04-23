// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_08
// File: PlayerHead.cs
// Description: 头部的控制逻辑,调用舌头的相关方法
// -------------------------------------------------

using UnityEngine;

namespace GamePlay.Player
{
    public delegate Vector3 DirectionLimit(Vector3 direction);
    
    public class PlayerHead : MonoBehaviour
    {
        [SerializeField] private PlayerTongue tongue;
        public PlayerTongue Tongue => tongue;
        [SerializeField] private Transform tongueRoot;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private SpriteRenderer spriteRenderer;
        public DirectionLimit DirectionLimit;
        private PlayerProperty _property;
        private void Start()
        {
            _property = playerController.Property;
        }

        private void Update()
        {
            UpdateDirection();
            if (Input.GetMouseButtonDown(0))
            {
                LaunchTongue();
            }
            if (Input.GetMouseButtonUp(0))
            {
                RetractTongue();
            }
            if (Input.GetMouseButtonDown(1))
            {
                InteractTongue();
            }
        }

        private void UpdateDirection()
        {
            if (!_property.HeadCanMove) return;
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var direction = mousePos - transform.position;
            direction.z = 0;
            direction.Normalize();
            if(DirectionLimit != null)
                direction = DirectionLimit(direction); // 确保此处输入的方向是归一化的
            // transform.right = Vector3.Lerp(transform.right, direction, 0.1f); 如果插值，落地时头部的转向会有错误
            transform.right = direction;
        }

        #region Tongue

        private void LaunchTongue()
        {
            if (!_property.HeadCanLaunch) return;
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