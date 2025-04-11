// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_08
// File: CameraController.cs
// Description:
// -------------------------------------------------

using GamePlay.Player;
using UnityEngine;

namespace Common.Manager
{
    public class CameraManager : MonoBehaviour
    {
        private Transform _target;
        private PlayerProperty _property;

        private void Start()
        {
            _target = GameManager.Instance.Player.transform;
            _property = GameManager.Instance.Player.Property;
        }

        private void Update()
        {
            if (_target == null) return;
            Vector3 pos = new Vector3(_target.position.x, _target.position.y, transform.position.z);

        }
    }
}