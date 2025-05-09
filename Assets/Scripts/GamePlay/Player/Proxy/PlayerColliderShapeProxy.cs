// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_04_10
// Description:
// -------------------------------------------------

using Common.FSM;
using UnityEngine;

namespace GamePlay.Player
{
    public class PlayerColliderShapeProxy : MonoBehaviour
    {
        [SerializeField] private PlayerController player;
        [SerializeField] private Collider2D collider0;
        [SerializeField] private Collider2D collider1;

        private void Start()
        {
            player.StateMachine.OnStateChanged += ChangeColliderShape;
        }

        private void ChangeColliderShape(HState from, HState to)
        {
            if (to.Name == "Hang" || to.Name == "OnPillar")
            {
                collider0.enabled = false;
                collider1.enabled = true;
            }
            else
            {
                collider0.enabled = true;
                collider1.enabled = false;
            }
        }
    }
}