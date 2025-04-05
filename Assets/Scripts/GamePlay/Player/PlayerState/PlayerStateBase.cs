// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_15
// File: PlayerStateBase.cs
// Description:
// -------------------------------------------------

using Common.FSM;
using GamePlay.Player.PlayerInput;
using UnityEngine;

namespace GamePlay.Player.PlayerState
{
    public abstract class PlayerStateBase : HState
    {
        protected PlayerProperty P;
        protected PlayerController Player;
        protected Rigidbody2D Rb;
        protected PlayerInputBase Input;

        public PlayerStateBase(PlayerController player, string name) : base(name)
        {
            Player = player;
            P = player.Property;
            Rb = player.Rb;
            Input = player.Input;
        }
    }
}