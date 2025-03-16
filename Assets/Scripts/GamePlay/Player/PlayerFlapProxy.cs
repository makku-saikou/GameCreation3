// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_16
// File: PlayerFlapProxy.cs
// Description:
// -------------------------------------------------

using System;
using UnityEngine;

namespace GamePlay.Player
{
    public class PlayerFlapProxy : MonoBehaviour
    {
        [SerializeField] private PlayerController playerController;
        private void Update()
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 direction = mousePosition - transform.position;
            direction.z = 0;
            direction.Normalize();
            // 什么奇怪的语法糖
            switch (direction.x)
            {
                case > 0 when !playerController.Property.isFacingRight:
                case < 0 when playerController.Property.isFacingRight:
                    playerController.Flip();
                    break;
            }
        }
    }
}