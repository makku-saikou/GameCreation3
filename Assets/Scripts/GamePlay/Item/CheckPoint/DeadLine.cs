// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_06_10
// Description:
// -------------------------------------------------

using Common.Manager;
using UnityEngine;

namespace GamePlay.Item.CheckPoint
{
    public class DeadLine : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            GameManager.Instance.PlayerDie();
        }
    }
}