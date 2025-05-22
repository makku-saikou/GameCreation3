// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_25
// Description:
// -------------------------------------------------

using GamePlay.Player;
using UnityEngine;

namespace GamePlay.Item.Target
{
    public class TargetBlock : MonoBehaviour, ITarget
    {
        [SerializeField] private float force = 10;
        public bool IsAdsorb => false;
        public Vector3 AdsorbPosition => transform.position;
        public Transform Root => transform;

        public void Interact(PlayerController playerController)
        {
            // Vector3 direction = transform.position - playerController.transform.position;
            // direction.Normalize();
            // playerController.Rb.AddForce(direction * force, ForceMode2D.Impulse);
        }
    }
}