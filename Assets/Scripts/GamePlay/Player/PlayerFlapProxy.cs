// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_16
// File: PlayerFlapProxy.cs
// Description:
// -------------------------------------------------

using UnityEngine;

namespace GamePlay.Player
{
    public class PlayerFlapProxy : MonoBehaviour
    {
        [SerializeField] private PlayerController playerController;
        private void Update()
        {
            // todo: 根据状态
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 direction = mousePosition - transform.position;
            direction.z = 0;
            direction.Normalize();
            // 什么奇怪的语法糖
            switch (direction.x)
            {
                case > 0 when !playerController.Property.IsFacingRight:
                case < 0 when playerController.Property.IsFacingRight:
                    playerController.Flip();
                    break;
            }
        }
    }
}