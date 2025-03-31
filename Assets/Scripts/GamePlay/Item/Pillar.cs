// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_3_31
// Description:
// -------------------------------------------------

using System;
using GamePlay.Player;
using UnityEngine;

namespace GamePlay.Item
{
    public class Pillar : MonoBehaviour
    {
        [SerializeField] private Transform highestPoint;
        public Transform HighestPoint => highestPoint;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                var player = other.GetComponent<PlayerController>();
                player.Property.maxClimbHeight = highestPoint.position.y;
            }
        }
    }
}