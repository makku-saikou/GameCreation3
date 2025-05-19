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
        [FoldoutGroup("公共属性")] [LabelText("常规情况跳跃力度")]
        public float jumpForce = 20f;
        
        [FoldoutGroup("公共属性")] [LabelText("跳跃次数")] [Tooltip("可以连续几段跳")]
        public int amountOfJump = 1;
    
        [FoldoutGroup("公共属性")] [LabelText("常规情况下重力缩放")]
        public float gravityScale = 5f;
    
        [FoldoutGroup("公共属性")] [LabelText("常规情况下最大速度")]
        public float commonXMaxSpeed = 10f;
    
        [FoldoutGroup("公共属性")] [LabelText("常规情况下最大速度")]
        public float commonYMaxSpeed = 60f;
    
        [FoldoutGroup("公共属性")] [LabelText("x最大速度插值恢复比率")]
        public float xMaxSpeedRecoverScale = 0.01f;
    
        [FoldoutGroup("公共属性")] [LabelText("y最大速度插值恢复比率")]
        public float yMaxSpeedRecoverScale = 0.05f;
    
        [FoldoutGroup("空中")] [LabelText("空中水平移动力度")]
        public float xForceInAir = 200f;
    
        [FoldoutGroup("空中")] [LabelText("提前松开空格，则会跳的更低")]
        public float variableJumpForce = 0.95f;
    
        [FoldoutGroup("空中")] [LabelText("预跳跃缓冲，详细问DZY")]
        public float preJumpBufferTime = 1f;
    
        [FoldoutGroup("悬挂")] [LabelText("悬挂且无输入时的空中阻尼")]
        public float hangDrag = 2f;
    
        [FoldoutGroup("悬挂")] [LabelText("悬挂时玩家输入的摇摆力")]
        public float hangSwayForce = 100f;
    
        [FoldoutGroup("悬挂")] [LabelText("悬挂时的重力缩放")]
        public float hangGravityScale = 12f;
    
        [FoldoutGroup("悬挂")] [LabelText("ws时舌头长度变化速度")]
        public float tongueLengthChangeSpeed = 1f;
    
        [FoldoutGroup("悬挂")] [LabelText("发射时玩家悬停速度缩放")]
        public float launchDragScale = 0.3f;
    
        [FoldoutGroup("悬挂")] [LabelText("悬挂时的跳跃力度")]
        public float hangJumpForce = 20f;
    
        [FoldoutGroup("悬挂")] [LabelText("退出悬挂补偿力")] [Tooltip("方向为切线，与水平面角度为90，补偿力为0，角度为0，力为该值")]
        public float hangForceCompensate = 10f;
    
        [FoldoutGroup("地面")] [LabelText("地面移动速度")]
        public float onGroundSpeed = 10f;
    
        [FoldoutGroup("地面")] [LabelText("地面检测高度")]
        public float groundCheckHeight = 0.1f;
    
        [FoldoutGroup("地面")] [LabelText("地面检测宽度")]
        public float groundCheckWidth = 0.5f;
    
        [FoldoutGroup("地面")] [LabelText("地面Layer")]
        public LayerMask groundLayer;
    
        [FoldoutGroup("地面")] [LabelText("跳跃缓冲时间")]
        public float jumpTimerSet = 0.15f;
    
        [FoldoutGroup("地面")] [LabelText("高跳时限")] [Tooltip("跳跃后在一定时间内按跳跃可以跳的更高")]
        public float jumpBufferTime = 0.5f;
    
        [FoldoutGroup("扒墙")] [LabelText("检测贴墙距离")]
        public float wallCheckRadius = 0.1f;
    
        [FoldoutGroup("扒墙")] [LabelText("滑墙速度")]
        public float wallSlideSpeed = 3f;
    
        [FoldoutGroup("扒墙")] [LabelText("滑墙速度插值恢复比率")]
        public float wallSpeedRecoverScale = 0.15f;
    
        [FoldoutGroup("扒墙")] [LabelText("蹬墙跳跃力度")]
        public float wallJumpForce = 120f;
    
        [FoldoutGroup("扒墙")] [LabelText("蹬墙跳跃缓冲时间")] [Tooltip("在该时间内不会再次进入扒墙状态")]
        public float wallJumpTimerSet = 0.15f;
    
        [FoldoutGroup("扒墙")] [LabelText("蹬墙跳跃方向")]
        public Vector2 wallJumpDirection = new(1f, 1f);
    
        [FoldoutGroup("扒墙")] [LabelText("脱离系数")] [Tooltip("有点无法量化，数值越大，脱离墙需要的时间越长,详细问LJH")]
        [Range(0, 0.99f)]
        public float wallExitCoefficient = 0.3f;
    
        [FoldoutGroup("下砸")] [LabelText("下砸下降速度")]
        public float smashVelocity = 30f;
    
        [FoldoutGroup("头和舌头")] [LabelText("舌头发射速度")]
        public float tongueSpeed = 40;
    
        [FoldoutGroup("头和舌头")] [LabelText("舌头回到嘴的速度")]
        public float retractSpeed = 100;
    
        [FoldoutGroup("头和舌头")] [LabelText("舌头最大长度")] [Tooltip("影响射程和悬挂时的最大长度")]
        public float tongueMaxLength = 8f;
    
        [FoldoutGroup("头和舌头")] [LabelText("舌头最小长度,影响时的最小长度")]
        public float tongueMinLength = 2;
    
        [FoldoutGroup("头和舌头")] [LabelText("舌头可以碰撞到的layer")]
        public List<LayerMask> targetLayers;
    
        [FoldoutGroup("头和舌头")] [LabelText("张嘴速度")]
        [Range(0, 0.1f)]
        public float openMouthSpeed = 0.05f;
    
        [FoldoutGroup("头和舌头")] [LabelText("闭嘴速度")]
        [Range(0, 0.1f)]
        public float closeMouthSpeed = 0.05f;
    
        [FoldoutGroup("爬杆")] [LabelText("爬杆攀爬速度")]
        public float climbPileSpeed = 5f;
    
        [FoldoutGroup("爬杆")] [LabelText("爬杆跳跃力度")]
        public float climbJumpForce = 100f;
    
        [FoldoutGroup("爬杆")] [LabelText("爬杆跳跃方向")]
        public Vector2 climbJumpDirection = new(1f, 1f);
    
        [FoldoutGroup("爬背景墙")] [LabelText("爬背景墙速度")]
        public float climbBackgroundSpeed = 10f;

        [FoldoutGroup("相机设置")] [LabelText("相机大小范围")] [MinMaxSlider(10, 50, true)] 
        public Vector2 cameraSize = new Vector2(16, 25);
        [FoldoutGroup("相机设置")] [LabelText("相机大小变化速度")] 
        public float lerpSpeed = 1f;
        [FoldoutGroup("相机设置")] [LabelText("相机大小阈值")] 
        public float cameraSizeThreshold = 20f;
        [FoldoutGroup("相机设置")] [LabelText("相机大小冻结时间")] 
        public float cameraSizeFreezeTime = 1f;
        [FoldoutGroup("相机设置")] [LabelText("玩家速度导致相机变化的阈值")] 
        public float playerSpeedThreshold = 10f;

        [FoldoutGroup("其他功能")] [Comment("地面上时最大抬头角度")] 
        public float onGroundUpLimit = 0.2f;
        [FoldoutGroup("其他功能")] [Comment("地面上时最大低头角度")] 
        public float onGroundDownLimit = 0.6f;
        
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
