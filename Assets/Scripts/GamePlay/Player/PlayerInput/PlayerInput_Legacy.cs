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
        public override float MovementInput => CanInput ? Input.GetAxis("Horizontal") : 0;
        public override bool JumpInputDown => CanInput && Input.GetButtonDown("Jump");
        public override bool JumpInput => CanInput && Input.GetButton("Jump");
        public override bool DownInput => CanInput && (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow));
        public override bool DownInputDown => CanInput && (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow));
        public override bool UpInput => CanInput && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow));

        public override Vector2 DirectionInput
        {
            get
            {
                if(!CanInput || (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0))
                {
                    return Vector2.zero;
                }

                return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
            }
        }

        public override bool InteractInput => CanInput && Input.GetKeyDown(KeyCode.E);

        public override Vector2 AttentionDirection
        {
            get
            {
                if (!CanInput || !Camera.main)
                {
                    return Vector2.zero;
                }
                var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var direction = mousePos - Player.Entity.position;
                direction.z = 0;
                direction.Normalize();
                return direction;
            }
        }

        public override bool LaunchDown => CanInput && Input.GetMouseButtonDown(0);
        public override bool LaunchUp => CanInput && Input.GetMouseButtonUp(0);
        public override bool ConnectInteractDown => CanInput && Input.GetMouseButtonDown(1);

        public PlayerInput_Legacy(PlayerController player) : base(player)
        {
            
        }
    }
}
