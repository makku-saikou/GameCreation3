// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_08
// File: PlayerHead.cs
// Description: 头部的控制逻辑,调用舌头的相关方法
// -------------------------------------------------
using UnityEngine;

namespace GamePlay.Player
{
    // TODO: 这个写法非常temp,之后我们要考虑InputSystem,如果玩家状态过多,考虑把PlayerController改成大状态机
    public class PlayerHead : MonoBehaviour
    {
        [SerializeField] private PlayerTongue playerTongue;
        [SerializeField] private Transform tongueRoot;
        public Transform TongueRoot => tongueRoot;
        // [SerializeField] private TongueChain tongueChain;
        public bool canMove;
        private void Update()
        {
            UpdateDirection();
            if (Input.GetMouseButtonDown(0))
            {
                LaunchTongue();
            }
            if (Input.GetMouseButtonUp(0))
            {
                RetractTongue();
            }
            if (Input.GetMouseButtonDown(1))
            {
                InteractTongue();
            }
            // tongueChain.SpringJoint2D.connectedAnchor = tongueRoot.position;
            // tongueChain.ResetJoint();
        }

        private void UpdateDirection()
        {
            if (!canMove) return;
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var direction = (mousePos - transform.position).normalized;
            direction.z = 0;
            transform.right = Vector3.Lerp(transform.right, direction, 0.1f);
        }

        private void LaunchTongue()
        {
            playerTongue.Launch(transform.position, transform.right);
        }

        private void RetractTongue()
        {
            playerTongue.Retract();
        }

        private void InteractTongue()
        {
            playerTongue.Interact();
        }
    }
}