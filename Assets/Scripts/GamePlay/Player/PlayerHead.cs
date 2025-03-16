// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_08
// File: PlayerHead.cs
// Description: 头部的控制逻辑,调用舌头的相关方法
// -------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Player
{
    public delegate Vector3 DirectionLimit(Vector3 direction);
    
    // TODO: 这个写法非常temp,之后我们要考虑InputSystem,如果玩家状态过多,考虑把PlayerController改成大状态机
    public class PlayerHead : MonoBehaviour
    {
        [SerializeField] private PlayerTongue playerTongue;
        [SerializeField] private Transform tongueRoot;
        [SerializeField] private PlayerController playerController;
        public DirectionLimit DirectionLimit;
        public Transform TongueRoot => tongueRoot;
        
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
        }

        private void UpdateDirection()
        {
            if (!canMove) return;
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var direction = mousePos - transform.position;
            direction.z = 0;
            direction.Normalize();
            if(DirectionLimit != null)
                direction = DirectionLimit(direction); // 确保此处输入的方向是归一化的
            transform.right = Vector3.Lerp(transform.right, direction, 0.1f);
        }

        #region Tongue

        private void LaunchTongue()
        {
            playerTongue.Launch(transform.position, transform.right);
        }

        public void RetractTongue()
        {
            playerTongue.Retract();
        }

        private void InteractTongue()
        {
            playerTongue.Interact();
        }

        #endregion
        //
        // #region Direction Limit
        //
        // public void AddDirectionLimit(string limitName, DirectionLimit directionLimit)
        // {
        //     _directionLimits[limitName] = directionLimit;
        // }
        //
        // public void RemoveDirectionLimit(string limitName)
        // {
        //     _directionLimits.Remove(limitName);
        // }
        //
        // public void SetDirectionLimit(string limitName)
        // {
        //     if (_directionLimits.ContainsKey(limitName))
        //     {
        //         _currentDirectionLimit = _directionLimits[limitName];
        //     }
        // }
        //
        // #endregion
    }
}