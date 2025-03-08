// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_08
// File: ConnectablePoint.cs
// Description:
// -------------------------------------------------

using GamePlay.Player;
using UnityEngine;

namespace GamePlay.Item
{
    public class ConnectablePoint : MonoBehaviour, IConnectable
    {
        [SerializeField] private float force = 10;
        public void Interact(PlayerController playerController)
        {
            Vector3 direction = transform.position - playerController.transform.position;
            direction.Normalize();
            // Vector3 direction = playerController.Head.transform.right;
            playerController.Rb.AddForce(direction * force, ForceMode2D.Impulse);
        }
    }
}