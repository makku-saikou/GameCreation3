// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li, zehua wu
// Date: 2025_03_08
// File: CameraManager.cs
// Description:
// -------------------------------------------------

using Cinemachine;
using GamePlay.Player;
using UnityEngine;

namespace Common.Manager
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private Vector2 maxMouseOffset = new Vector2(2f, 1f);
        [SerializeField] private float mouseOffsetDamping = 1f;

        private static PlayerController Player => GameManager.Instance.Player;
        private static Vector2 CameraSize => Player.Config.cameraSize;
        private static float LerpSpeed => Player.Config.lerpSpeed;
        private static float CameraSizeThreshold => Player.Config.cameraSizeThreshold;
        private static float CameraSizeFreezeTime => Player.Config.cameraSizeFreezeTime;
        private static float PlayerSpeedThreshold => Player.Config.playerSpeedThreshold;

        private float _sizeFreezeCounter;
        private CinemachineCameraOffset _cameraOffset;

        private void Start()
        {
            virtualCamera.Follow = GameManager.Instance.Player.CameraPoint.transform;
            _cameraOffset = virtualCamera.GetComponent<CinemachineCameraOffset>();
        }

        private void Update()
        {
            DynamicCameraSize();
            MouseOffset();
        }

        private void DynamicCameraSize()
        {
            if (_sizeFreezeCounter > 0)
            {
                _sizeFreezeCounter -= Time.deltaTime;
                return;
            }

            var velocity = Player.Rb.velocity;
            // var maxVelocity = new Vector2(Player.Config.commonXMaxSpeed, Player.Config.commonYMaxSpeed);
            var maxVelocity = Player.Config.airMaxSpeed;
            var targetSize = CameraSize.x + (CameraSize.y - CameraSize.x) * (velocity.magnitude / maxVelocity.magnitude);
            var currentSize = virtualCamera.m_Lens.OrthographicSize;

            if (targetSize > CameraSizeThreshold && currentSize > CameraSizeThreshold) _sizeFreezeCounter = CameraSizeFreezeTime;
            if (velocity.magnitude < PlayerSpeedThreshold && Mathf.Abs(currentSize - CameraSize.x) < 0.1) return;

            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(currentSize, targetSize, Time.deltaTime * LerpSpeed);
        }

        private void MouseOffset()
        {
            if (!_cameraOffset) return;
            var mousePos = new Vector2(
                Input.mousePosition.x / Screen.width * 2 - 1,
                Input.mousePosition.y / Screen.height * 2 - 1
                );

            var targetOffset = new Vector2(
                Mathf.Clamp(mousePos.x * maxMouseOffset.x, -maxMouseOffset.x, maxMouseOffset.x),
                Mathf.Clamp(mousePos.y * maxMouseOffset.y, -maxMouseOffset.y, maxMouseOffset.y)
            );

            var currentOffset = _cameraOffset.m_Offset;
            var offset = Vector2.Lerp(currentOffset, targetOffset, mouseOffsetDamping * Time.deltaTime);
            _cameraOffset.m_Offset = new Vector3(offset.x, offset.y, currentOffset.z);
        }
    }
}
