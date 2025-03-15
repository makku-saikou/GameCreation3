// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_15
// File: OnGround.cs
// Description:
// -------------------------------------------------

using Common.FSM;
using UnityEngine;

namespace GamePlay.Player.PlayerState
{
    public class OnGround : HState
    {
        private PlayerProperty _property;
        public OnGround(string name, PlayerProperty property) : base(name)
        {
            _property = property;
        }

        public OnGround(PlayerProperty property)
        {
            _property = property;
        }

        public override void UpdateCallback(float deltaTime)
        {
            base.UpdateCallback(deltaTime);
            
        }
        
        
    }
}