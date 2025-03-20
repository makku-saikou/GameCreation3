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

        [SerializeField] private PlayerProperty property;
        public PlayerProperty Property => property;

        private HStateMachine _stateMachine;
        public HStateMachine StateMachine => _stateMachine;
        
        [SerializeField] private SpriteRenderer spriteRenderer;
        public SpriteRenderer SpriteRenderer => spriteRenderer;
        
        private Rigidbody2D _rb;
        public Rigidbody2D Rb => _rb;
        
        [SerializeField]private Transform groundCheckPoint; // 地面检测点

        private void Awake()
        {
            // temp
            Init();
        }

        private void Update()
        {
            CheckState();
            StateMachine.UpdateCallback(Time.deltaTime);
        }
        
        private void FixedUpdate()
        {
            StateMachine.FixedUpdateCallback();
        }

        /// <summary>
        /// 原则是，我们确保关于Player的逻辑模块都在PlayerController的Init之后初始化
        /// </summary>
        private void Init()
        {
            _rb = GetComponent<Rigidbody2D>();
            property.Init();
            
            // 定义整个状态机
            // 状态
            OnGroundState onGroundState = new OnGroundState(this, "OnGround");
            AirState airState = new AirState(this, "Air");
            HangState hangState = new HangState(this, "Hang");
            
            // 转移
            HTransition jump = new HTransition("Jump", onGroundState, airState);
            jump.OnCheck += () => !property.IsGrounded;
            onGroundState.AddTransition("Jump", airState, () => !property.IsGrounded);
            
            HTransition land = new HTransition("Land", airState, onGroundState);
            land.OnCheck += () => property.IsGrounded;
            airState.AddTransition(land);
            
            HTransition connect = new HTransition("Connect", airState, hangState);
            connect.OnCheck += () => property.IsConnecting;
            airState.AddTransition(connect);
            
            HTransition hangJump = new HTransition("HangJump", hangState, airState);
            hangJump.OnCheck += () => !property.IsConnecting;
            hangState.AddTransition(hangJump);
            
            // 初始化状态机
            _stateMachine = new HStateMachine(onGroundState);
            _stateMachine.AddState(airState);
            _stateMachine.AddState(hangState);
        }

        [Obsolete]
        private void Init(PlayerProperty property) { }
        
        public void Flip()
        {
            if (property.IsWallSliding || !property.CanFlip) return;
            property.FacingDirection *= -1;
            property.IsFacingRight = !property.IsFacingRight;
            transform.Rotate(0, 180, 0);
        }
        
        /// <summary>
        /// 检查状态的替换由玩家本体负责，而不放入状态中
        /// </summary>
        private void CheckState()
        {
            property.IsGrounded = 
                Physics2D.OverlapCircle(groundCheckPoint.position, property.groundCheckRadius, property.groundLayer);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Pillar"))
            {
                property.IsOnPillar = true;
            }
        }
        
        private void OnCollisionExit2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Pillar"))
            {
                property.IsOnPillar = false;
            }
        }

        // private void OnDrawGizmos()
        // {
        //     Gizmos.DrawWireSphere(property.groundCheckPoint.position, property.groundCheckRadius);
        // }
    }
}
