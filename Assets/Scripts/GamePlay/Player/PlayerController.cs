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
using PurpleFlowerCore.Utility;
using UnityEngine;

// 考虑到玩家状态较多，各种子状态需要考虑有无连接或其他情况，舌头本身也有多种状态
// 我们使用并行的两个状态机，一个用于玩家，一个用于舌头，两状态机之间的影响和数据传递通过独立拉出的玩家数据类实现
// 现在的实现中,我们暂不考虑蹬墙或蹭墙,如果有,未来可以算作空中子状态
namespace GamePlay.Player
{
    public class PlayerController : MonoBehaviour
    {
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
        [RO] public bool isTouchingWall;                       // 是否贴墙，由Physics2D判定
        [RO] public bool isWallSliding;                        // 是否滑墙
        [RO] public bool checkVariableJump;                    // 当成功跳跃时被激活，若跳跃期间松开空格，则会施加额外的向下的力
        [RO] public bool canNormalJump;                        // 是否可以进行普通跳跃
        [RO] public bool canMove = true;                       // 是否可以移动
        [RO] public bool canFlip = true;                       // 是否可以转向
        [RO] public float jumpTimer;                           // 跳跃计时器，提供输入提前量，优化下一次跳跃的手感
        
        private Rigidbody2D _rb;
        public Rigidbody2D Rb => _rb;
        [SerializeField] private PlayerHead _head;
        public PlayerHead Head => _head;

        [SerializeField] private PlayerProperty playerProperty;
        public PlayerProperty Property => playerProperty;

        private HStateMachine stateMachine;
        public HStateMachine StateMachine => stateMachine;
        
        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            amountOfJumpLeft = amountOfJump;
        }

        private void Update()
        {
            CheckInput();
            CheckMovementState();
            CheckJumpState();
            
            StateMachine.UpdateCallback(Time.deltaTime);
        }
        
        private void FixedUpdate()
        {
            ApplyMovement();
            CheckSurroundings();
            
            StateMachine.FixedUpdateCallback();
        }

        private void Init()
        {
            playerProperty = new PlayerProperty();
            OnGround onGround = new OnGround(playerProperty);
            // 注意这里的状态机和PFC的状态机名称类似，在这个项目里我们暂时使用Common。FSM的状态机
            stateMachine = new HStateMachine(onGround);
        }

        [Obsolete]
        private void Init(PlayerProperty property)
        {
            
        }
        
        private void CheckInput()
        {
            movementInput = Input.GetAxisRaw("Horizontal");

            if (Input.GetButtonDown("Jump"))
            {
                if (isGrounded || (amountOfJumpLeft > 0 && !isTouchingWall))
                    NormalJump();
                else
                    jumpTimer = jumpTimerSet;
            }

            if (checkVariableJump && !Input.GetButton("Jump"))
            {
                checkVariableJump = false;
                var velocity = _rb.velocity;
                velocity = new Vector2(velocity.x, velocity.y * variableJumpHeightMultiplier);
                _rb.velocity = velocity;
            }
        }

        private void CheckMovementState()
        {
            if ((isFacingRight && movementInput < 0) || 
                (!isFacingRight && movementInput > 0)) 
                Flip();
            
            isWalking = Math.Abs(_rb.velocity.x) > 0.01f; // rigidbody的速度在移动时会有一个极小的值，故为>0.01，其他小值也可，令人费解的bug
        }
        
        private void CheckJumpState()
        {
            if (isGrounded && _rb.velocity.y <= 0.01f) // 着陆时
            {
                amountOfJumpLeft = amountOfJump;
                checkVariableJump = false;
            }

            if (isTouchingWall) checkVariableJump = false;

            canNormalJump = amountOfJumpLeft > 0;
            
            if (jumpTimer > 0)
            {
                if (isGrounded) 
                    NormalJump();
                
                jumpTimer -= Time.deltaTime;
            }
        }
        
        private void CheckSurroundings()
        {
            isGrounded = 
                Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
        }
        
        private void ApplyMovement()
        {
            if (!isGrounded && !isWallSliding && movementInput == 0)
            {
                // 当在空中且没有输入时，会受到空气阻力
                var velocity = _rb.velocity;
                velocity = new Vector2(velocity.x * airDragMultiplier, velocity.y);
                _rb.velocity = velocity;
            }
            else if (canMove)
            {
                // 正常移动
                if(movementInput != 0)
                    _rb.velocity = new Vector2(movementSpeed * movementInput, _rb.velocity.y);
            }
        }
        
        private void NormalJump()
        {
            if (!canNormalJump) return;
            
            _rb.velocity = new Vector2(_rb.velocity.x, jumpForce);
                
            amountOfJumpLeft--;
            jumpTimer = 0;
            checkVariableJump = true;
        }

        private void Flip()
        {
            if (isWallSliding || !canFlip) return;
            facingDirection *= -1;
            isFacingRight = !isFacingRight;
            transform.Rotate(0, 180, 0);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
        }
    }
}
