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
using PurpleFlowerCore.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GamePlay.Item
{
    public class ColorBlock_Swim : MapElement
    {
        [Title("颜色块-游泳")]
        [SerializeField] private EPlayerColor color = EPlayerColor.None;
        [SerializeField] private Collider2D collider2D;
        [SerializeField] [LabelText("当颜色和玩家不同时（墙）")] private Color originColor;
        [SerializeField] [LabelText("当颜色和玩家相同时（池）")] private Color poolColor;

        private Tilemap _tilemap;

        protected override void Init()
        {
            _tilemap = GetComponent<Tilemap>();
            if (!_tilemap)
                PFCLog.Error("ColorBlock_Swim", "Tilemap is not found on the ColorBlock_Swim object.");
            Player.Property.OnColorChanged += OnPlayerColorChanged;
            OnPlayerColorChanged(EPlayerColor.None, Player.Property.CurrentColor);
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

            _tilemap.color = to == color ? poolColor : originColor;
        }
        
        private void OnPlayerColorChangedInThis(EPlayerColor from, EPlayerColor to)
        {
            PFCLog.Debug("ColorBlock", $"player color {to.ToString()}");
            GameManager.Instance.PlayerDie();
        }
    }
}
