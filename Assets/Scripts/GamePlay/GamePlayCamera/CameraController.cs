// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_08
// File: CameraController.cs
// Description:
// -------------------------------------------------

using System;
using UnityEngine;

namespace GamePlay.GamePlayCamera
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform target;

        private void Update()
        {
            if (target == null) return;
            transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
        }
    }
}