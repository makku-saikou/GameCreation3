// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_4_21
// Description:
// -------------------------------------------------

using GamePlay.Player;
using PurpleFlowerCore;
using UnityEngine;

namespace GamePlay.Item
{
    public class ColorBlock : MonoBehaviour
    {
        [SerializeField] private EPlayerColor color = EPlayerColor.Orange;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            var player = other.GetComponent<PlayerController>();
            player.Property.CanOnColorBlock = player.Property.CurrentColor == color;
            player.Property.OnColorChanged += OnPlayerColorChanged;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            var player = other.GetComponent<PlayerController>();
            player.Property.CanOnColorBlock = false;
            player.Property.OnColorChanged -= OnPlayerColorChanged;
        }

        private void OnPlayerColorChanged()
        {
            PFCLog.Debug("ColorBlock", "Player's color changed");
        }
    }
}