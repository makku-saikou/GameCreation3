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
        protected PlayerController Player;
        protected PlayerProperty Property=>Player.Property;
        protected PlayerHead Head => Player.Head;
        protected PlayerConfig Config => Player.Config;
        protected Rigidbody2D Rb => Player.Rb;
        protected PlayerInputBase Input => Player.Input;
        protected PlayerTongue Tongue => Player.Head.Tongue;
        protected Transform Entity => Player.Entity;
        public PlayerStateBase(PlayerController player, string name) : base(name)
        {
            Player = player;
        }
    }
}