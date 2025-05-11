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
        [SerializeField] [MinMaxSlider(10, 50)] private Vector2 cameraSize = new Vector2(16, 25);
        [SerializeField] private float lerpSpeed = 1f;

        private PlayerController _player;
        private Vector2 _maxVelocity;

        private void Start()
        {
            virtualCamera.Follow = GameManager.Instance.Player.CameraPoint.transform;
            _player = GameManager.Instance.Player;
            _maxVelocity = new Vector2(_player.Config.commonXMaxSpeed, _player.Config.commonYMaxSpeed);
        }

        private void Update()
        {
            var velocity = _player.Rb.velocity;
            var targetSize = cameraSize.x + (cameraSize.y - cameraSize.x) * (velocity.magnitude / _maxVelocity.magnitude);
            targetSize = Mathf.Clamp(targetSize, cameraSize.x, cameraSize.y);
            var currentSize = virtualCamera.m_Lens.OrthographicSize;
            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(currentSize, targetSize, Time.deltaTime * lerpSpeed);
        }
    }
}
