// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_08
// Description:
// -------------------------------------------------

using GamePlay.Player;
using UnityEngine;

namespace GamePlay.Item
{
    public class TargetPoint : MonoBehaviour, ITarget
    {
        [SerializeField] private float force = 10;
        public bool IsAdsorb => true;
        public Vector3 AdsorbPosition => transform.position;

        public void Interact(PlayerController playerController)
        {
            Vector3 direction = transform.position - playerController.transform.position;
            direction.Normalize();
            // Vector3 direction = playerController.Head.transform.right;
            playerController.Rb.AddForce(direction * force, ForceMode2D.Impulse);
        }
    }
}