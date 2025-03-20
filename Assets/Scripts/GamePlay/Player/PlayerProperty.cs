// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_14
// File: PlayerProperty.cs
// Description:
// -------------------------------------------------

using PurpleFlowerCore;
using UnityEngine;

namespace GamePlay.Player
{
    [CreateAssetMenu(fileName = "PlayerProperty", menuName = "Data/PlayerProperty")]
    [Configurable]
    public class PlayerProperty : ScriptableObject
    {
        // todo: 属性分类,剥离部分属性
        [Header("公共属性")]
        public float movementSpeed = 10f;                   // 移动速度
        public float jumpForce = 16f;                       // 跳跃力度
        public int amountOfJump = 1;                        // 跳跃次数（可以连续几段跳）

        [Header("空中")]
        public float xMixSpeedInAir = 20f;                  // 空中水平最大移动速度
        // public float fallMultiplier = 0.95f;                 // 下落时的空气阻力
        // public float variableJumpHeightMultiplier = 0.5f;   // 提前松开空格，则会跳的更低
        
        [Header("悬挂")]
        [Range(0,5)]public float hangDrag = 2f;       //  悬挂且无输入时的空中阻尼
        public float hangSwayForce = 50f;                   // 悬挂时玩家输入的摇摆力
        public float hangGravityScale = 12f;                // 悬挂时的重力
        
        [Header("地面")]
        public float groundCheckRadius = 0.3f;              // 地面检测圆半径
        public LayerMask groundLayer;                       // 地面Layer
        public float jumpTimerSet = 0.15f;                  // 跳跃缓冲时间
        
        public float MovementInput { get; set; }               // 输入方向
        public int AmountOfJumpLeft { get; set; }              // 剩余跳跃次数
        public int FacingDirection { get; set; }                  // _isFacingRight的数值形式，方便计算
        public bool IsFacingRight { get; set; }                // 是否正面向右边
        public bool IsWalking { get; set; }                           // 是否在行走，动画参数
        public bool IsGrounded { get; set; }                          // 是否在地面上，由Physics2D判定
        public bool IsWallSliding { get; set; }                       // 是否滑墙
        public bool CanNormalJump { get; set; }                       // 是否可以进行普通跳跃
        public bool CanMove { get; set; }                      // 是否可以移动
        public bool CanFlip { get; set; }                      // 是否可以转向
        public bool IsConnecting { get; set; }
        public float ConnectAngle { get; set; }                       // 连接点与玩家的连线与竖直方向的夹角,角度制,当玩家在连接点左侧时为负
        // [RO] public bool checkVariableJump;                    // 当成功跳跃时被激活，若跳跃期间松开空格，则会施加额外的向下的力
        // [RO] public bool isTouchingWall;                       // 是否贴墙，由Physics2D判定
        
        public void Init()
        {
            FacingDirection = 1;
            AmountOfJumpLeft = amountOfJump;
            IsFacingRight = true;
            CanMove = true;
            CanFlip = true;
        }
    }
}