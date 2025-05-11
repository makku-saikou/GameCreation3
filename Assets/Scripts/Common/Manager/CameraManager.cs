// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_08
// File: CameraController.cs
// Description:
// -------------------------------------------------

using System;
using Cinemachine;
using GamePlay.Player;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Common.Manager
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera virtualCamera;

        private static PlayerController Player => GameManager.Instance.Player;
        private static Vector2 CameraSize => Player.Config.cameraSize;
        private static float LerpSpeed => Player.Config.lerpSpeed;

        private void Start()
        {
            virtualCamera.Follow = GameManager.Instance.Player.CameraPoint.transform;
        }

        private void Update()
        {
            var maxVelocity = new Vector2(Player.Config.commonXMaxSpeed, Player.Config.commonYMaxSpeed);
            var velocity = Player.Rb.velocity;
            var targetSize = CameraSize.x + (CameraSize.y - CameraSize.x) * (velocity.magnitude / maxVelocity.magnitude);
            targetSize = Mathf.Clamp(targetSize, CameraSize.x, CameraSize.y);
            var currentSize = virtualCamera.m_Lens.OrthographicSize;
            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(currentSize, targetSize, Time.deltaTime * LerpSpeed);
        }
    }
}
