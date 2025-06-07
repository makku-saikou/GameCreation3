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
    public class ColorBlock_Swim : MapElement
    {
        [Title("颜色块-游泳")]
        [SerializeField] private EPlayerColor color = EPlayerColor.None;
        [SerializeField] private Collider2D collider2D;

        protected override void Init()
        {
            Player.Property.OnColorChanged += OnPlayerColorChanged;
            collider2D.isTrigger = Player.Property.CurrentColor == color;
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
            GameManager.Instance.PlayerDie();
        }
    }
}