// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_4_21
// Description:
// -------------------------------------------------

using System;
using Common.Manager;
using GamePlay.Player;
using PurpleFlowerCore;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePlay.Item
{
    public class ColorBlock_Swim : MonoBehaviour
    {
        [Title("颜色块-穿梭")]
        [SerializeField] private EPlayerColor color = EPlayerColor.None;
        [SerializeField] private Collider2D collider2D;
        // [SerializeField] private SpriteRenderer spriteRenderer;

        private void Start()
        {
            GameManager.Instance.Player.Property.OnColorChanged += OnPlayerColorChanged;
            // switch (color)
            // {
            //     case EPlayerColor.None:
            //         PFCLog.Warning("颜色块", "颜色块没有设置颜色");
            //         spriteRenderer.color = Color.white;
            //         break;
            //     case EPlayerColor.Green:
            //         spriteRenderer.color = Color.green;
            //         break;
            //     case EPlayerColor.Red:
            //         spriteRenderer.color = Color.red;
            //         break;
            //     case EPlayerColor.Blue:
            //         spriteRenderer.color = Color.blue;
            //         break;
            //     default:
            //         throw new ArgumentOutOfRangeException();
            // }
            collider2D.isTrigger = GameManager.Instance.Player.Property.CurrentColor == color;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            var player = other.GetComponent<PlayerController>();
            player.Property.CanOnSwimColorBlock = true;
            player.Property.OnColorChanged += OnPlayerColorChangedInThis;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            var player = other.GetComponent<PlayerController>();
            player.Property.CanOnSwimColorBlock = false;
            player.Property.OnColorChanged -= OnPlayerColorChangedInThis;
        }

        private void OnPlayerColorChanged(EPlayerColor from, EPlayerColor to)
        {
            collider2D.isTrigger = to == color;
            if(collider2D.isTrigger)
                gameObject.layer = LayerMask.NameToLayer("Default");
            else
                gameObject.layer = LayerMask.NameToLayer("Ground");
        }
        
        private void OnPlayerColorChangedInThis(EPlayerColor from, EPlayerColor to)
        {
            PFCLog.Debug("ColorBlock", $"player color {to.ToString()}");
        }
    }
}