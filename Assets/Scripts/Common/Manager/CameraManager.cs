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
        private static float CameraSizeThreshold => Player.Config.cameraSizeThreshold;
        private static float CameraSizeFreezeTime => Player.Config.cameraSizeFreezeTime;
        private static float PlayerSpeedThreshold => Player.Config.playerSpeedThreshold;

        private float _sizeFreezeCounter;

        private void Start()
        {
            virtualCamera.Follow = GameManager.Instance.Player.CameraPoint.transform;
        }

        private void Update()
        {
            if (_sizeFreezeCounter > 0)
            {
                _sizeFreezeCounter -= Time.deltaTime;
                return;
            }

            var velocity = Player.Rb.velocity;
            var maxVelocity = new Vector2(Player.Config.commonXMaxSpeed, Player.Config.commonYMaxSpeed);
            var targetSize = CameraSize.x + (CameraSize.y - CameraSize.x) * (velocity.magnitude / maxVelocity.magnitude);
            var currentSize = virtualCamera.m_Lens.OrthographicSize;

            if (targetSize > CameraSizeThreshold && currentSize > CameraSizeThreshold) _sizeFreezeCounter = CameraSizeFreezeTime;
            if (velocity.magnitude < PlayerSpeedThreshold && Mathf.Abs(currentSize - CameraSize.x) < 0.1) return;

            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(currentSize, targetSize, Time.deltaTime * LerpSpeed);
        }
    }
}
