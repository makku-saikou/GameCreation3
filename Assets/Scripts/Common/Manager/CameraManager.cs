// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_08
// File: CameraController.cs
// Description:
// -------------------------------------------------

using Cinemachine;
using UnityEngine;

namespace Common.Manager
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        private void Start()
        {
            virtualCamera.Follow = GameManager.Instance.Player.CameraPoint.transform;
        }
    }
}