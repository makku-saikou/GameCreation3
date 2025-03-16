// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_08
// File: PlayerController.cs
// Description: 玩家角色主要控制逻辑
// -------------------------------------------------

using System;
using Common.FSM;
using GamePlay.Player.PlayerState;
using UnityEngine;

// 考虑到玩家状态较多，各种子状态需要考虑有无连接或其他情况，舌头本身也有多种状态
// 我们使用并行的两个状态机，一个用于玩家，一个用于舌头，两状态机之间的影响和数据传递通过独立拉出的玩家数据类实现
// 现在的实现中,我们暂不考虑蹬墙或蹭墙,如果有,未来可以算作空中子状态
namespace GamePlay.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerHead head;
        public PlayerHead Head => head;

        [SerializeField] private PlayerProperty playerProperty;
        public PlayerProperty Property => playerProperty;

        private HStateMachine _stateMachine;
        public HStateMachine StateMachine => _stateMachine;
        
        private Rigidbody2D _rb;
        public Rigidbody2D Rb => _rb;

        private void Awake()
        {
            // temp
            Init();
        }

        private void Update()
        {
            StateMachine.UpdateCallback(Time.deltaTime);
        }
        
        private void FixedUpdate()
        {
            StateMachine.FixedUpdateCallback();
        }

        /// <summary>
        /// 原则是，我们确保关于Player的逻辑模块都在Init之后初始化
        /// </summary>
        private void Init()
        {
            _rb = GetComponent<Rigidbody2D>();
            playerProperty.Init();
            PlayerStateBase playerStateBase = new PlayerStateBase(this);
            // 注意这里的状态机和PFC的状态机名称类似，在这个项目里我们暂时使用Common。FSM的状态机
            _stateMachine = new HStateMachine(playerStateBase);
        }

        [Obsolete]
        private void Init(PlayerProperty property)
        {
            
        }
        
        public void Flip()
        {
            if (playerProperty.isWallSliding || !playerProperty.canFlip) return;
            playerProperty.facingDirection *= -1;
            playerProperty.isFacingRight = !playerProperty.isFacingRight;
            transform.Rotate(0, 180, 0);
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(playerProperty.groundCheckPoint.position, playerProperty.groundCheckRadius);
        }
    }
}
