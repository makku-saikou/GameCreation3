// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_4_5
// Description:
// -------------------------------------------------

using UnityEngine;

namespace GamePlay.Player.PlayerInput
{
    public class PlayerInput_Legacy : PlayerInputBase
    {
        public override float MovementInput => Input.GetAxis("Horizontal");
        public override bool JumpInputDown => Input.GetButtonDown("Jump");
        public override bool JumpInput => Input.GetButton("Jump");
        public override bool DownInput => Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
        public override bool UpInput => Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        public override Vector2 DirectionInput => new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }
}