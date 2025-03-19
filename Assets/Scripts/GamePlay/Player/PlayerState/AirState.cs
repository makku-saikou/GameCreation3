// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_16
// Description:
// -------------------------------------------------

using Common.FSM;
using PurpleFlowerCore;
using PurpleFlowerCore.Resource;
using UnityEngine;

namespace GamePlay.Player.PlayerState
{
    public class AirState : PlayerStateBase
    {
        public AirState(PlayerController player, string name) : base(player, name) { }

        public override void EnterCallback(HState prev)
        {
            base.EnterCallback(prev);
            PFCLog.Debug("Enter Air State");
            AddressableModule addressableModule = new AddressableModule();
            addressableModule.Load<Sprite>("Body1", sprite =>
            {
                _player.SpriteRenderer.sprite = sprite.Result;
            });
        }

        public override void ExitCallback(HState next)
        {
            base.ExitCallback(next);
            PFCLog.Debug("Exit Air State");
        }

        public override void UpdateCallback(float deltaTime)
        {
            base.UpdateCallback(deltaTime);
            CheckInput();
            // ConnectCheckPower();

        }
        
        public override void FixedUpdateCallback()
        {
            base.FixedUpdateCallback();
            AppleMovement();
            // DiminishPower();
            // AddForce();
        }

        private void CheckInput()
        {
            _p.movementInput = Input.GetAxisRaw("Horizontal");
            
            if(_p.isConnecting && Input.GetButtonDown("Jump"))
            {
                // todo: 解耦
                _player.Head.RetractTongue();
                AirJump();
            }
        }
    
        private void AppleMovement()
        {
            // todo: 添加子状态
            if(!_p.isConnecting)
            {
                if (!_p.isWallSliding && _p.movementInput != 0)
                {
                    // var velocity = _rb.velocity;
                    // velocity = new Vector2(_p.movementSpeed * _p.movementInput, velocity.y);
                    // _rb.velocity = velocity;
                    _rb.AddForce(new Vector2(10 * _p.movementInput, 0), ForceMode2D.Force);
                    var velocity = _rb.velocity;
                    if (Mathf.Abs(velocity.x) > 40)
                    {
                        velocity = new Vector2(10 * _p.movementInput, velocity.y);
                        _rb.velocity = velocity;
                    }
                    
                }
                else if (_p.isWallSliding && _p.movementInput == 0)
                {
                    var velocity = _rb.velocity;
                    velocity = new Vector2(velocity.x * _p.fallMultiplier, velocity.y);
                    _rb.velocity = velocity;
                }
            }
            else
            {
                // todo: 这仿狗勾的效果也太难做了
                
                // 能量法 - 解决正负问题
                // float v = Mathf.Sqrt(_p.s1 * (_p.power - Mathf.Abs(Mathf.Cos(Mathf.Deg2Rad * _p.connectAngle)) * _p.s2));
                // Debug.Assert(v >= 0);
                // float vx = v * Mathf.Abs(Mathf.Cos(Mathf.Deg2Rad * _p.connectAngle));
                // float vy = v * Mathf.Sin(Mathf.Deg2Rad * _p.connectAngle);
                // PFCLog.Debug("AirState", _p.connectAngle, v, vx, vy);
                // _rb.velocity = new Vector2(vx, vy);
                // if (_p.movementInput != 0)
                // {
                //     vx = _p.movementSpeed * _p.movementInput;
                // }
                // else
                // {
                //     vx = 0;
                // }
                
                // 简单难受法
                // if(_p.movementInput != 0)
                // {
                //     var velocity = _rb.velocity;
                //     velocity = new Vector2(_p.movementSpeed * _p.movementInput, velocity.y);
                //     _rb.velocity = velocity;
                // }
                
                // Unity物理法
                // temp
                _player.Rb.drag = _p.movementInput == 0 ? 5 : 0;
                if (_p.movementInput != 0)
                {
                    _player.Rb.AddForce(new Vector2(50 * _p.movementInput, 0), ForceMode2D.Force);
                }
                
            }
        }

        private void ConnectCheckPower()
        {
            if(!_p.isConnecting) return;
            if(_p.movementInput !=0 && _p.movementInput * _p.connectAngle > 0)
            {
                _p.power += Time.deltaTime * 20;
            }
            else if(_p.movementInput != 0 && _p.movementInput * _p.connectAngle < 0)
            {
                _p.power -= Time.deltaTime * 30;
            }
            else
            {
                _p.power -= Time.deltaTime * 10;
            }
                
            _p.power = Mathf.Clamp(_p.power, Mathf.Abs(Mathf.Cos(Mathf.Deg2Rad * _p.connectAngle)) * _p.s2 + 1, _p.maxPower);
        }
        
        private void DiminishPower()
        {
            if(!_p.isConnecting) return;
            if (_p.movementInput != 0) return;
            _p.power -= Time.deltaTime * 10;
            if (_p.power <= 0) _p.power = 0;
            // _p.power = Mathf.Clamp(_p.power, Mathf.Abs(Mathf.Cos(Mathf.Deg2Rad * _p.connectAngle)) * _p.s2 + 1, _p.maxPower);
        }
        
        private void DiminishVelocity()
        {
            if(!_p.isConnecting) return;
            if (_p.movementInput != 0) return;
            var velocity = _rb.velocity;
            velocity = new Vector2(velocity.x * _p.airHangMultiplier, velocity.y);
            _rb.velocity = velocity;
        }
        
        private void AirJump()
        {
            // if (!_p.canNormalJump) return;
            
            _rb.velocity += new Vector2(0, _p.jumpForce);
            
            _p.amountOfJumpLeft--;
            // _p.checkVariableJump = true;
        }
    }
}