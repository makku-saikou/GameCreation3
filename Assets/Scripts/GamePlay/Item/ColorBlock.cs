// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_4_21
// Description:
// -------------------------------------------------

using System;
using Common.Manager;
using GamePlay.Player;
using UnityEngine;

namespace GamePlay.Item
{
    public class ColorBlock : MonoBehaviour
    {
        [SerializeField] private EPlayerColor color = EPlayerColor.Orange;
        [SerializeField] private Collider2D collider2D;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private void Start()
        {
            GameManager.Instance.Player.Property.OnColorChanged += OnPlayerColorChanged;
            switch (color)
            {
                case EPlayerColor.Orange:
                    spriteRenderer.color = Color.yellow;
                    break;
                case EPlayerColor.Green:
                    spriteRenderer.color = Color.green;
                    break;
                case EPlayerColor.Red:
                    spriteRenderer.color = Color.red;
                    break;
                case EPlayerColor.Blue:
                    spriteRenderer.color = Color.blue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            collider2D.isTrigger = GameManager.Instance.Player.Property.CurrentColor == color;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            var player = other.GetComponent<PlayerController>();
            // player.Property.CanOnColorBlock = player.Property.CurrentColor == color;
            player.Property.CanOnColorBlock = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            var player = other.GetComponent<PlayerController>();
            player.Property.CanOnColorBlock = false;
        }

        private void OnPlayerColorChanged(EPlayerColor from, EPlayerColor to)
        {
            collider2D.isTrigger = to == color;
        }
    }
}