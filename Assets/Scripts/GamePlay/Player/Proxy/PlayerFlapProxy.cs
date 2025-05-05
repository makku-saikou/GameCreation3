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
        private PlayerProperty Property => player.Property;
        private Rigidbody2D Rb => player.Rb;
        private PlayerFlap _onGround;
        private PlayerFlap _air;
        private PlayerFlap _wall;
        private PlayerFlap _alwaysRight;
        private PlayerFlap _none;
        private PlayerFlap _keep;
        private bool _rightBuffer;

        private void Start()
        {
            _onGround = () =>
            {
                if (Property.IsLaunching || Property.IsRetracting || Property.IsConnecting) return;
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
                switch (Rb.velocity.x)
                {
                    case > 0 when !Property.IsFacingRight:
                    case < 0 when Property.IsFacingRight:
                        Flip();
                        break;
                }
            };

            _wall = () =>
            {
                switch (Property.IsFacingRight)
                {
                    case false when Property.IsRightWall:
                    case true when !Property.IsRightWall:
                        Flip();
                        break;
                }
            };

            _alwaysRight = () =>
            {
                if (!Property.IsFacingRight)
                    Flip();
            };

            _none = () => { };

            _keep = () =>
            {
                if (_rightBuffer && !Property.IsFacingRight)
                    Flip();
                else if (!_rightBuffer && Property.IsFacingRight)
                    Flip();
            };
            
             player.StateMachine.OnStateChanged += CheckFlap;
        }
        
        private void OnDisable()
        {
            player.StateMachine.OnStateChanged -= CheckFlap;
        }
        
        private void CheckFlap(HState from, HState to)
        {
            if(to.Name == "Hang")
            {
                _rightBuffer = player.Head.Tongue.TonguePoint.position.x > player.transform.position.x;
            }
            if (to.Name == "OnGround" || to.Name == "OnWall")
            {
                ResetTransform();
            }
            player.PlayerFlap = to.Name switch
            {
                "OnGround" => _onGround,
                "Air" => _air,
                "OnWall" => _wall,
                "Hang" => _keep,
                "OnBackground" => _alwaysRight,
                _ => _none
            };
        }


        private void Flip()
        {
            if (!player.Property.CanFlip) return;
            Debug.Log("Flip");
            player.Property.IsFacingRight = !player.Property.IsFacingRight;
            // player.Entity.Rotate(0, 180, 0);
            player.Entity.localScale = new Vector3(-1 * player.Entity.localScale.x, player.Entity.localScale.y, player.Entity.localScale.z);
            player.Head.transform.localScale = new Vector3(-1 * player.Head.transform.localScale.x, -1 * player.Head.transform.localScale.y, 1);
            // player.Head.transform.localScale = new Vector3(-1 * player.Head.transform.localScale.x, -1 *player.Head.transform.localScale.y, player.Head.transform.localScale.z);
        }

        private void ResetTransform()
        {
            player.Entity.rotation = Quaternion.identity;
            player.Entity.localScale = new Vector3(Property.FacingDirection, 1, 1);
        }
    }
}