// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_16
// File: PlayerFlapProxy.cs
// Description:
// -------------------------------------------------

using System;
using Common.FSM;
using UnityEngine;

namespace GamePlay.Player
{
    public class PlayerFlapProxy : MonoBehaviour
    {
        [SerializeField] private PlayerController player;
        private PlayerFlap _onGround;
        private PlayerFlap _air;

        private void Start()
        {
            _onGround = () =>
            {
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

            _air = () => { };
            
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
                _ => player.PlayerFlap
            };
        }
        
        
        public void Flip()
        {
            if (!player.Property.CanFlip) return;
            player.Property.IsFacingRight = !player.Property.IsFacingRight;
            player.Entity.Rotate(0, 180, 0);
            player.Head.transform.localScale = new Vector3(1, -1 * player.Head.transform.localScale.y, 1);
        }
    }
}