// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_14
// File: PlayerProperty.cs
// Description:
// -------------------------------------------------

using System;
using PurpleFlowerCore.Utility;
using UnityEngine;

namespace GamePlay.Player
{
    [Serializable]
    public class PlayerProperty
    {
        // private HStateMachine _stateMachine;
        // public HStateMachine StateMachine => _stateMachine;
        //
        // public PlayerProperty(HStateMachine sm)
        // {
        //     _stateMachine = sm;
        // }
        
        [Header("BaseMovement")]
        public float movementSpeed = 10f;                   // 移动速度
        public float jumpForce = 16f;                       // 跳跃力度
        public int amountOfJump = 1;                        // 跳跃次数（可以连续几段跳）

        [Header("JumpOptimize")]
        public float airDragMultiplier = 0.95f;             // 如果在空中没有输入，则会更快落下
        public float variableJumpHeightMultiplier = 0.5f;   // 提前松开空格，则会跳的更低
        public float jumpTimerSet = 0.15f;                  // 跳跃缓冲时间
        
        [Header("GroundCheck")]
        public Transform groundCheckPoint;                  // 地面检测点
        public float groundCheckRadius = 0.3f;              // 地面检测圆半径
        public LayerMask groundLayer;                       // 地面Layer
        
        [RO] public float movementInput;                       // 输入方向
        [RO] public int amountOfJumpLeft;                      // 剩余跳跃次数
        [RO] public int facingDirection = 1;                   // _isFacingRight的数值形式，方便计算
        [RO] public bool isFacingRight = true;                 // 是否正面向右边
        [RO] public bool isWalking;                            // 是否在行走，动画参数
        [RO] public bool isGrounded;                           // 是否在地面上，由Physics2D判定
        // [RO] public bool isTouchingWall;                       // 是否贴墙，由Physics2D判定
        [RO] public bool isWallSliding;                        // 是否滑墙
        // [RO] public bool checkVariableJump;                    // 当成功跳跃时被激活，若跳跃期间松开空格，则会施加额外的向下的力
        [RO] public bool canNormalJump;                        // 是否可以进行普通跳跃
        [RO] public bool canMove = true;                       // 是否可以移动
        [RO] public bool canFlip = true;                       // 是否可以转向
        
        public void Init()
        {
            amountOfJumpLeft = amountOfJump;
        }
    }
}