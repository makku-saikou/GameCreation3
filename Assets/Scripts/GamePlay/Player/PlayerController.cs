// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_08
// File: PlayerController.cs
// Description: 玩家角色主要控制逻辑
// -------------------------------------------------

using System;
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
        public float wallSlidingSpeed = 1.5f;               // 滑墙速度

        [Header("JumpOptimize")]
        public float airDragMultiplier = 0.95f;             // 如果在空中没有输入，则会更快落下
        public float variableJumpHeightMultiplier = 0.5f;   // 提前松开空格，则会跳的更低
        public float jumpTimerSet = 0.15f;                  // 跳跃缓冲时间
        public float freezeTimerSet = 0.1f;                 // 蹬墙跳时的冻结时间（优化操作手感）
        public float wallJumpTimerSet = 0.5f;               // 蹬墙跳缓冲时间（防止连续上跳）
        
        [Header("GroundCheck")]
        public Transform groundCheckPoint;                  // 地面检测点
        public float groundCheckRadius = 0.3f;              // 地面检测圆半径
        public LayerMask groundLayer;                       // 地面Layer
        
        private float _movementInput;                       // 输入方向
        private int _amountOfJumpLeft;                      // 剩余跳跃次数
        private int _facingDirection = 1;                   // _isFacingRight的数值形式，方便计算
        private bool _isFacingRight = true;                 // 是否正面向右边
        private bool _isWalking;                            // 是否在行走，动画参数
        private bool _isGrounded;                           // 是否在地面上，由Physics2D判定
        private bool _isTouchingWall;                       // 是否贴墙，由Physics2D判定
        private bool _isTouchingLedge;                      // 是否贴墙角，由Physics2D判定
        private bool _isWallSliding;                        // 是否滑墙
        private bool _checkVariableJump;                    // 当成功跳跃时被激活，若跳跃期间松开空格，则会施加额外的向下的力
        private bool _canNormalJump;                        // 是否可以进行普通跳跃
        private bool _canMove;                              // 是否可以移动
        private bool _canFlip;                              // 是否可以转向
        
        private float _jumpTimer;                           // 跳跃计时器，提供输入提前量，优化下一次跳跃的手感
        private float _freezeTimer;                         // 在触发蹬墙跳前冻结移动和转向
        private float _wallJumpTimer;                       // 防止在同一面墙上连续上跳
        private int _lastWallJumpDirection;                 // 记录上次蹬墙跳的方向，用处同上
        private bool _hasWallJump;
        
        private Rigidbody2D _rb;
        public Rigidbody2D Rb => _rb;
        [SerializeField] private PlayerHead _head;
        public PlayerHead Head => _head;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _amountOfJumpLeft = amountOfJump;
        }

        private void Update()
        {
            CheckInput();
            CheckMovementState();
            CheckJumpState();
        }
        
        private void FixedUpdate()
        {
            ApplyMovement();
            CheckSurroundings();
        }
        
        private void CheckInput()
        {
            _movementInput = Input.GetAxisRaw("Horizontal");

            if (Input.GetButtonDown("Jump"))
            {
                if (_isGrounded || (_amountOfJumpLeft > 0 && !_isTouchingWall))
                    NormalJump();
                else
                    _jumpTimer = jumpTimerSet;
            }

            // 冻结一小段时间的移动与转向，使得蹬墙跳的触发更加容易
            if (Input.GetButtonDown("Horizontal") &&
                _isTouchingWall && !_isGrounded && Math.Abs(_movementInput - _facingDirection) > 0) 
            {
                _canMove = false;
                _canFlip = false;
                _freezeTimer = freezeTimerSet;
            }
            
            if (_freezeTimer >= 0)
            {
                _freezeTimer -= Time.deltaTime;
                if (_freezeTimer <= 0)
                {
                    _canMove = true;
                    _canFlip = true;
                }
            }

            if (_checkVariableJump && !Input.GetButton("Jump"))
            {
                _checkVariableJump = false;
                var velocity = _rb.velocity;
                velocity = new Vector2(velocity.x, velocity.y * variableJumpHeightMultiplier);
                _rb.velocity = velocity;
            }
        }

        private void CheckMovementState()
        {
            if ((_isFacingRight && _movementInput < 0) || 
                (!_isFacingRight && _movementInput > 0)) 
                Flip();
            
            _isWalking = Math.Abs(_rb.velocity.x) > 0.01f; // rigidbody的速度在移动时会有一个极小的值，故为>0.01，其他小值也可，令人费解的bug
        }
        
        private void CheckJumpState()
        {
            if (_isGrounded && _rb.velocity.y <= 0.01f) // 着陆时
            {
                _amountOfJumpLeft = amountOfJump;
                _checkVariableJump = false;
            }

            if (_isTouchingWall) _checkVariableJump = false;

            _canNormalJump = _amountOfJumpLeft > 0;
            
            if (_jumpTimer > 0)
            {
                if (_isGrounded) 
                    NormalJump();
                
                _jumpTimer -= Time.deltaTime;
            }
        }
        
        private void CheckSurroundings()
        {
            _isGrounded = 
                Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
        }
        
        private void ApplyMovement()
        {
            if (!_isGrounded && !_isWallSliding && _movementInput == 0)
            {
                // 当在空中且没有输入时，会受到空气阻力
                var velocity = _rb.velocity;
                velocity = new Vector2(velocity.x * airDragMultiplier, velocity.y);
                _rb.velocity = velocity;
            }
            else if (_canMove)
            {
                // 正常移动
                if(_movementInput != 0)
                    _rb.velocity = new Vector2(movementSpeed * _movementInput, _rb.velocity.y);
            }
        }
        
        private void NormalJump()
        {
            if (!_canNormalJump) return;
            
            _rb.velocity = new Vector2(_rb.velocity.x, jumpForce);
                
            _amountOfJumpLeft--;
            _jumpTimer = 0;
            _checkVariableJump = true;
        }

        private void Flip()
        {
            if (_isWallSliding || !_canFlip) return;
            _facingDirection *= -1;
            _isFacingRight = !_isFacingRight;
            transform.Rotate(0, 180, 0);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
        }
    }
}
