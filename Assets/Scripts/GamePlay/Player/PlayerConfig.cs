// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_14
// File: PlayerProperty.cs
// Description:
// -------------------------------------------------

using System.Collections.Generic;
using Common.Attribute;
using Common.Manager;
using PurpleFlowerCore;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePlay.Player
{
    /// <summary>
    /// 玩家属性的配置类和数据类，我们使用字段配置数
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerProperty", menuName = "Data/PlayerProperty")]
    [Configurable("Player")]
    public class PlayerConfig : ScriptableObject
    {
        [Header("公共属性")]
        [Comment("常规情况跳跃力度")]public float jumpForce = 20f; 
        [Comment("跳跃次数（可以连续几段跳）")]public int amountOfJump = 1;                        
        [Comment("常规情况下重力缩放")]public float gravityScale = 5f;                     
        [Comment("常规情况下最大速度")]public float commonXMaxSpeed = 10f;                 
        [Comment("常规情况下最大速度")]public float commonYMaxSpeed = 60f;                 
        [Comment("x最大速度插值恢复比率")]public float xMaxSpeedRecoverScale = 0.01f;         
        [Comment("y最大速度插值恢复比率")]public float yMaxSpeedRecoverScale = 0.05f;         

        [Header("空中")]
        [Comment("空中水平移动力度")]public float xForceInAir = 200f;                     
        // public float fallMultiplier = 0.95f;             // 下落时的空气阻力
        [Comment("提前松开空格，则会跳的更低")]public float variableJumpForce = 0.95f;             
        [Comment("预跳跃缓冲，详细问DZY")]public float preJumpBufferTime = 1f;
        
        [Header("悬挂")]
        [Comment("悬挂且无输入时的空中阻尼")]public float hangDrag = 2f;             
        [Comment("悬挂时玩家输入的摇摆力")]public float hangSwayForce = 100f;                   
        [Comment("悬挂时的重力缩放")]public float hangGravityScale = 12f;                
        [Comment("ws时舌头长度变化速度")]public float tongueLengthChangeSpeed = 1f;
        [Comment("发射时玩家悬停速度缩放")]public float launchDragScale = 0.3f;
        [Comment("悬挂时的跳跃力度")]public float hangJumpForce = 20f;
        [Comment("退出悬挂补偿力，方向为切线，与水平面角度为90，补偿力为0，角度为0，力为该值")]
        public float hangForceCompensate = 10f;
        
        [Header("地面")]
        [Comment("地面移动速度")]public float onGroundSpeed = 10f;                   
        [Comment("地面检测高度")]public float groundCheckHeight = 0.1f;              
        [Comment("地面检测宽度")]public float groundCheckWidth = 0.5f;               
        [Comment("地面Layer")]public LayerMask groundLayer;                       
        [Comment("跳跃缓冲时间")]public float jumpTimerSet = 0.15f;                  
        [Comment("跳跃后在一定时间内按跳跃可以跳的更高")] public float jumpBufferTime = 0.5f;
        
        [Header("扒墙")]
        [Comment("检测贴墙距离")]public float wallCheckRadius = 0.1f;                
        [Comment("滑墙速度")]public float wallSlideSpeed = 3f;                   
        [Comment("滑墙速度插值恢复比率")]public float wallSpeedRecoverScale = 0.15f;          
        [Comment("蹬墙跳跃力度")]public float wallJumpForce = 120f;                    
        [Comment("蹬墙跳跃缓冲时间，在该时间内不会再次进入扒墙状态")]public float wallJumpTimerSet = 0.15f;              
        [Comment("蹬墙跳跃方向")]public Vector2 wallJumpDirection = new(1f, 1f);
        
        [Header("下砸")]
        [Comment("下砸下降速度")]public float smashVelocity = 30f;                   
        
        [Header("舌头")]
        [Comment("舌头发射速度")]public float tongueSpeed = 40;
        [Comment("舌头回到嘴的速度")]public float retractSpeed = 100;
        [Comment("舌头最大长度,影响射程和悬挂时的最大长度")]public float tongueMaxLength = 8f;
        [Comment("舌头最小长度,影响时的最小长度")]public float tongueMinLength = 2;
        [Comment("舌头可以碰撞到的layer")] public List<LayerMask> targetLayers;
        
        [Header("爬杆")]
        [Comment("爬杆攀爬速度")]public float climbPileSpeed = 5f;                       
        
        [Header("爬背景墙")]
        [Comment("爬背景墙速度")]public float climbBackgroundSpeed = 10f;
        
        [Header("其他功能")]
        [Comment("地面上时最大抬头角度")][SerializeField] public float onGroundUpLimit = 0.2f;
        [Comment("地面上时最大低头角度")][SerializeField] public float onGroundDownLimit = 0.6f;
#if UNITY_EDITOR
        public bool IsPlayerConfig => this == GameManager.Instance?.Player.Config;
        [ShowIf("@!IsPlayerConfig && UnityEditor.EditorApplication.isPlaying")]
        [GUIColor(1f, 0f, 0f)]
        [InfoBox("此配置不是当前玩家的配置！", InfoMessageType.Error)]
        [ShowIf("@!IsPlayerConfig&& UnityEditor.EditorApplication.isPlaying")]
        [Button]
        public void ChangePlayerCurrentConfigToThis()
        {
            GameManager.Instance.Player.Config = this;
        }
#endif
    }
}
