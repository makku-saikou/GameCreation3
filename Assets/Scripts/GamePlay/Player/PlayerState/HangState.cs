// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_20
// Description:
// -------------------------------------------------

using Common.FSM;
using PurpleFlowerCore;
using PurpleFlowerCore.Resource;
using UnityEngine;

namespace GamePlay.Player.PlayerState
{
    public class HangState : PlayerStateBase
    {
        public HangState(PlayerController player, string name) : base(player, name) { }
        public override void EnterCallback(HState prev)
        {
            base.EnterCallback(prev);
            PFCLog.Debug("Enter Hang State");
            AddressableModule addressableModule = new AddressableModule();
            addressableModule.Load<Sprite>("Body0", sprite =>
            {
                _player.SpriteRenderer.sprite = sprite.Result;
            });
            
            _rb.gravityScale = 10;
            // var p = playerController.Property;
            // p.connectAngle = Vector2.SignedAngle(Vector2.down,
            //     playerController.transform.position - transform.position);
            // p.power = playerController.Rb.velocity.SqrMagnitude() / p.s1 + p.s2 * Mathf.Abs(Mathf.Cos(Mathf.Deg2Rad * p.connectAngle));
        }

        public override void UpdateCallback(float deltaTime)
        {
            base.UpdateCallback(deltaTime);
            _p.movementInput = Input.GetAxisRaw("Horizontal");
            
            if(_p.isConnecting && Input.GetButtonDown("Jump"))
            {
                // todo: 解耦
                _player.Head.RetractTongue();
                HangJump();
            }
        }

        public override void FixedUpdateCallback()
        {
            base.FixedUpdateCallback();
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
            _player.Rb.drag = _p.movementInput == 0 ? _p.airHangDrag : 0;
            if (_p.movementInput != 0)
            {
                _player.Rb.AddForce(new Vector2(50 * _p.movementInput, 0), ForceMode2D.Force);
            }
        }

        public override void ExitCallback(HState next)
        {
            base.ExitCallback(next);
            _rb.gravityScale = 5;
            _player.Rb.drag = 0;
        }

        private void HangJump()
        {
            _rb.velocity += new Vector2(0, _p.jumpForce);
            
            // _p.amountOfJumpLeft--;
        }
        
        // private void ConnectCheckPower()
        // {
        //     if(!_p.isConnecting) return;
        //     if(_p.movementInput !=0 && _p.movementInput * _p.connectAngle > 0)
        //     {
        //         _p.power += Time.deltaTime * 20;
        //     }
        //     else if(_p.movementInput != 0 && _p.movementInput * _p.connectAngle < 0)
        //     {
        //         _p.power -= Time.deltaTime * 30;
        //     }
        //     else
        //     {
        //         _p.power -= Time.deltaTime * 10;
        //     }
        //         
        //     _p.power = Mathf.Clamp(_p.power, Mathf.Abs(Mathf.Cos(Mathf.Deg2Rad * _p.connectAngle)) * _p.s2 + 1, _p.maxPower);
        // }
        //
        // private void DiminishPower()
        // {
        //     if(!_p.isConnecting) return;
        //     if (_p.movementInput != 0) return;
        //     _p.power -= Time.deltaTime * 10;
        //     if (_p.power <= 0) _p.power = 0;
        //     // _p.power = Mathf.Clamp(_p.power, Mathf.Abs(Mathf.Cos(Mathf.Deg2Rad * _p.connectAngle)) * _p.s2 + 1, _p.maxPower);
        // }
        //
        // private void DiminishVelocity()
        // {
        //     if(!_p.isConnecting) return;
        //     if (_p.movementInput != 0) return;
        //     var velocity = _rb.velocity;
        //     velocity = new Vector2(velocity.x * _p.airHangMultiplier, velocity.y);
        //     _rb.velocity = velocity;
        // }
    }
}