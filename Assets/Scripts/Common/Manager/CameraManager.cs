// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_08
// File: CameraController.cs
// Description:
// -------------------------------------------------

using UnityEngine;

namespace Common.Manager
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private Transform target;

        private void Update()
        {
            if (target == null) return;
            transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
        }
    }
}