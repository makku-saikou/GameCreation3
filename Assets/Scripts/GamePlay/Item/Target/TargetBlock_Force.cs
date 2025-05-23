// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_05_24
// Description:
// -------------------------------------------------

using GamePlay.Player;
using UnityEngine;

namespace GamePlay.Item.Target
{
    public class TargetBlock_Force : MonoBehaviour, ITarget
    {
        [SerializeField] private float force = 200;
        public bool IsAdsorb => false;
        public Vector3 AdsorbPosition { get; }
        public bool Interact(PlayerController playerController)
        {
            Vector3 direction = playerController.Entity.up;
            direction.Normalize();
            playerController.Rb.AddForce(direction * force, ForceMode2D.Impulse);
            return true;
        }

        public Transform Root => transform;
    }
}