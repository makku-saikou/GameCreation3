// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_16
// Description:
// -------------------------------------------------

using Common.FSM;
using UnityEngine;

namespace GamePlay.Player
{
    //todo: 将翻转功能代理出来的可行性
    public class PlayerFlapProxy : MonoBehaviour
    {
        [SerializeField] private PlayerController player;
        private PlayerProperty _property;
        private Rigidbody2D _rb;
        private PlayerFlap _onGround;
        private PlayerFlap _air;
        private PlayerFlap _wall;
        private PlayerFlap _alwaysRight;
        private PlayerFlap _none;

        private void Start()
        {
            _property = player.Property;
            _rb = player.Rb;
            _onGround = () =>
            {
                if (_property.IsLaunching || _property.IsRetracting || _property.IsConnecting) return;
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3 direction = mousePosition - transform.position;
                direction.z = 0;
                direction.Normalize();
                // 什么奇怪的语法糖
                switch (direction.x)
                {
                    case > 0 when !player.Property.IsFacingRight:
                    case < 0 when player.Property.IsFacingRight:
                        Flip();
                        break;
                }
            };

            _air = () =>
            {
                switch (_rb.velocity.x)
                {
                    case > 0 when !_property.IsFacingRight:
                    case < 0 when _property.IsFacingRight:
                        Flip();
                        break;
                }
            };

            _wall = () =>
            {
                switch (_property.IsFacingRight)
                {
                    case false when _property.IsRightWall:
                    case true when !_property.IsRightWall:
                        Flip();
                        break;
                }
            };

            _alwaysRight = () =>
            {
                if (!_property.IsFacingRight)
                    Flip();
            };

            _none = () => { };
            
             player.StateMachine.OnStateChanged += CheckFlap;
        }
        
        private void OnDisable()
        {
            player.StateMachine.OnStateChanged -= CheckFlap;
        }
        
        private void CheckFlap(HState from, HState to)
        {
            player.PlayerFlap = to.Name switch
            {
                "OnGround" => _onGround,
                "Air" => _air,
                "OnWall" => _wall,
                "Hang" => _alwaysRight,
                "OnBackground" => _alwaysRight,
                _ => _none
            };
        }


        private void Flip()
        {
            if (!player.Property.CanFlip) return;
            player.Property.IsFacingRight = !player.Property.IsFacingRight;
            player.Entity.Rotate(0, 180, 0);
            player.Head.transform.localScale = new Vector3(1, -1 * player.Head.transform.localScale.y, 1);
        }
    }
}