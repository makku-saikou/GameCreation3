using System;
using UnityEngine;

namespace GamePlay.Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("BaseMovement")]
        public float movementSpeed = 10f;                   // 移动速度
        public float jumpForce = 16f;                       // 跳跃力度
        public int amountOfJump = 2;                        // 跳跃次数（可以连续几段跳）
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

        [Header("WallCheck")] 
        public Transform wallCheckPoint;                    // 墙体检测点
        public Transform ledgeCheckPoint;                   // 墙角检测点
        public float wallCheckDistance;                     // 墙体检测距离

        [Header("WallJump")]
        public Vector2 wallJumpDirection;                   // 蹬墙跳方向
        public float wallJumpForce = 20f;                   // 蹬墙跳力度

        [Header("LedgeClimb")]
        // 爬墙结束后设置的位置偏移量
        public float ledgeClimbXOffset1 = 0.3f;
        public float ledgeClimbYOffset1 = 0f;
        public float ledgeClimbXOffset2 = 0.5f;
        public float ledgeClimbYOffset2 = 2f;
        
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
        private bool _canWallJump;                          // 是否可以进行蹬墙跳
        private bool _canMove;                              // 是否可以移动
        private bool _canFlip;                              // 是否可以转向
        
        private float _jumpTimer;                           // 跳跃计时器，提供输入提前量，优化下一次跳跃的手感
        private float _freezeTimer;                         // 在触发蹬墙跳前冻结移动和转向
        private float _wallJumpTimer;                       // 防止在同一面墙上连续上跳
        private int _lastWallJumpDirection;                 // 记录上次蹬墙跳的方向，用处同上
        private bool _hasWallJump;
        
        private bool _isLedgeClimb;                         // 是否正在爬墙
        private bool _ledgeDetected;                        // 是否检测到了墙角
        private Vector2 _ledgePosBot;                       //
        private Vector2 _ledgePos1;
        private Vector2 _ledgePos2;
        
        private Rigidbody2D _rb;
        // private Animator _ani;
        // private static readonly int IsWalking = Animator.StringToHash("IsWalking");
        // private static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
        // private static readonly int YVelocity = Animator.StringToHash("yVelocity");
        // private static readonly int IsWallSliding = Animator.StringToHash("IsWallSliding");
        // private static readonly int CanClimbLedge = Animator.StringToHash("CanClimbLedge");

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            // _ani = GetComponent<Animator>();
            _amountOfJumpLeft = amountOfJump;
            wallJumpDirection.Normalize();
        }

        private void Update()
        {
            CheckInput();
            CheckMovementState();
            CheckJumpState();
            CheckWallSlideState();
            CheckLedgeClimb();
            UpdateAnimations();
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
            _canWallJump = _isTouchingWall;
            
            if (_jumpTimer > 0)
            {
                if (!_isGrounded && _isTouchingWall && _movementInput != 0 &&
                    Math.Abs(_movementInput - _facingDirection) > 0) 
                    WallJump();
                else if (_isGrounded) 
                    NormalJump();
                
                _jumpTimer -= Time.deltaTime;
            }

            if (_wallJumpTimer > 0)
            {
                if (Math.Abs(_movementInput + _lastWallJumpDirection) < 0.01  && _hasWallJump)
                {
                    _hasWallJump = false;
                    _rb.velocity = new Vector2(_rb.velocity.x, -1.5f);
                }
                else if (_wallJumpTimer <= 0) 
                    _hasWallJump = false;
                else 
                    _wallJumpTimer -= Time.deltaTime;
            }
        }

        private void CheckWallSlideState()
        {
            // 当我们接触墙壁且输入指向墙壁时，进入滑墙状态
            _isWallSliding = _isTouchingWall && 
                             Math.Abs(_movementInput - _facingDirection) < 0.01f &&
                             _rb.velocity.y < -0.01f &&
                             !_isLedgeClimb;
        }

        private void CheckLedgeClimb()
        {
            if (_ledgeDetected && !_isLedgeClimb)
            {
                _isLedgeClimb = true;
                if (_isFacingRight)
                {
                    _ledgePos1 = new Vector2(
                        Mathf.Floor(_ledgePosBot.x + wallCheckDistance) - ledgeClimbXOffset1,
                        Mathf.Floor(_ledgePosBot.y) + ledgeClimbYOffset1);
                    _ledgePos2 = new Vector2(
                        Mathf.Floor(_ledgePosBot.x + wallCheckDistance) + ledgeClimbXOffset2,
                        Mathf.Floor(_ledgePosBot.y) + ledgeClimbYOffset2);
                }
                else
                {
                    _ledgePos1 = new Vector2(
                        Mathf.Ceil(_ledgePosBot.x - wallCheckDistance) + ledgeClimbXOffset1,
                        Mathf.Floor(_ledgePosBot.y) + ledgeClimbYOffset1);
                    _ledgePos2 = new Vector2(
                        Mathf.Floor(_ledgePosBot.x - wallCheckDistance) - ledgeClimbXOffset2,
                        Mathf.Floor(_ledgePosBot.y) + ledgeClimbYOffset2);
                }
                _canMove = false;
                _canFlip = false;
                
                // _ani.SetBool(CanClimbLedge, true);
            }

            if (_isLedgeClimb) 
                transform.position = _ledgePos1;
        }
        
        private void CheckSurroundings()
        {
            _isGrounded = 
                Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);

            _isTouchingWall =
                Physics2D.Raycast(wallCheckPoint.position, transform.right, wallCheckDistance, groundLayer);
            _isTouchingLedge = 
                Physics2D.Raycast(ledgeCheckPoint.position, transform.right, wallCheckDistance, groundLayer);

            if (_isTouchingWall && !_isTouchingLedge && !_ledgeDetected)
            {
                _ledgeDetected = true;
                _ledgePosBot = wallCheckPoint.position;
            }
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
                // 正常移动
                _rb.velocity = new Vector2(movementSpeed * _movementInput, _rb.velocity.y);
        
            if (_isWallSliding && _rb.velocity.y < -wallSlidingSpeed) // 限制滑墙状态的速度
                _rb.velocity = new Vector2(_rb.velocity.x, -wallSlidingSpeed);
        }
        
        private void NormalJump()
        {
            if (!_canNormalJump) return;
            
            _rb.velocity = new Vector2(_rb.velocity.x, jumpForce);
                
            _amountOfJumpLeft--;
            _jumpTimer = 0;
            _checkVariableJump = true;
        }

        private void WallJump()
        {
            if (!_canWallJump) return;
            
            _rb.velocity = new Vector2(_rb.velocity.x, 0);
            _amountOfJumpLeft = amountOfJump;
            _amountOfJumpLeft--;
                
            var forceToAdd = new Vector2(
                wallJumpForce * wallJumpDirection.x * _movementInput, 
                wallJumpForce * wallJumpDirection.y);
            _rb.AddForce(forceToAdd, ForceMode2D.Impulse);
                
            _jumpTimer = 0;
            _freezeTimer = 0;
            _isWallSliding = false;
            _checkVariableJump = true;
            _canMove = true;
            _canFlip = true;

            _wallJumpTimer = wallJumpTimerSet;
            _lastWallJumpDirection = -_facingDirection;
            _hasWallJump = true;
        }

        private void Flip()
        {
            if (_isWallSliding || !_canFlip) return;
            _facingDirection *= -1;
            _isFacingRight = !_isFacingRight;
            transform.Rotate(0, 180, 0);
        }
        
        private void UpdateAnimations()
        {
            // _ani.SetBool(IsWalking, _isWalking);
            // _ani.SetBool(IsGrounded, _isGrounded);
            // _ani.SetBool(IsWallSliding, _isWallSliding);
            // _ani.SetFloat(YVelocity, _rb.velocity.y);
        }

        public void FinishLedgeClimb()
        {
            _isLedgeClimb = false;
            transform.position = _ledgePos2;
            _canMove = true;
            _canFlip = true;
            _ledgeDetected = false;
            // _ani.SetBool(CanClimbLedge, false);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
            var wallCheckPosition = wallCheckPoint.position;
            Gizmos.DrawLine(wallCheckPosition, new Vector3(wallCheckPosition.x + wallCheckDistance, wallCheckPosition.y, wallCheckPosition.z));
            var ledgeCheckPosition = ledgeCheckPoint.position;
            Gizmos.DrawLine(ledgeCheckPosition, new Vector3(ledgeCheckPosition.x + wallCheckDistance, ledgeCheckPosition.y, ledgeCheckPosition.z));
        }
    }
}
