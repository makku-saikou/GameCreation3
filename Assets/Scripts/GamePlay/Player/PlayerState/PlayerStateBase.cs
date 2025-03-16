// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_15
// File: PlayerStateBase.cs
// Description:
// -------------------------------------------------

using Common.FSM;
using UnityEngine;

namespace GamePlay.Player.PlayerState
{
    public abstract class PlayerStateBase : HState
    {
        protected PlayerProperty _p;
        protected PlayerController _player;
        protected Rigidbody2D _rb;

        public PlayerStateBase(PlayerController player, string name) : base(name)
        {
            _player = player;
            _p = player.Property;
            _rb = player.Rb;
        }

        // protected PlayerStateBase(PlayerController player)
        // {
        //     
        // }
    }
}