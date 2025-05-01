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
        protected PlayerProperty Property;
        protected PlayerHead Head;
        protected PlayerConfig Config;
        protected PlayerController Player;
        protected Rigidbody2D Rb;
        protected PlayerInputBase Input;
        protected PlayerTongue Tongue;

        public PlayerStateBase(PlayerController player, string name) : base(name)
        {
            Player = player;
            Property = player.Property;
            Rb = player.Rb;
            Input = player.Input;
            Tongue = player.Head.Tongue;
            Config = player.Config;
            Head = player.Head;
        }
    }
}