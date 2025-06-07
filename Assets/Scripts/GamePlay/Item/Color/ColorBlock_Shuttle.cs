// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_05_23
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
    public class ColorBlock_Shuttle : MapElement
    {
        [Title("颜色块-穿梭")]
        [SerializeField] private EPlayerColor color = EPlayerColor.None;
        [SerializeField] private Collider2D collider2D;
        private bool _canShuttle;
        private bool _playerIn;
        protected override void Init()
        {
            Player.Property.OnColorChanged += OnPlayerColorChanged;
            UpdateState();
        }

        private void Update()
        {
            CheckCanShuttle();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            var player = other.GetComponent<PlayerController>();
            player.Property.OnColorChanged += OnPlayerColorChangedInThis;
            player.Property.IsShuttle = true;
            _playerIn = true;
            player.OnCollisionEnter += OnPlayerBumpInThis;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            var player = other.GetComponent<PlayerController>();
            player.Property.OnColorChanged -= OnPlayerColorChangedInThis;
            player.Property.IsShuttle = false;
            _playerIn = false;
            player.OnCollisionEnter -= OnPlayerBumpInThis;
        }

        private void OnPlayerColorChanged(EPlayerColor from, EPlayerColor to)
        {
            UpdateState();
        }
        
        private void OnPlayerColorChangedInThis(EPlayerColor from, EPlayerColor to)
        {
            PFCLog.Debug("ColorBlock", $"player color {to.ToString()}");
            GameManager.Instance.PlayerDie();
        }

        private void OnPlayerBumpInThis(Collision2D _)
        {
            PFCLog.Debug("ColorBlock", "player bumped when shuttling");
            GameManager.Instance.PlayerDie();
        }

        private void UpdateState()
        {

            collider2D.isTrigger = _canShuttle && GameManager.Instance.Player.Property.CurrentColor == color;
        }
        
        private void CheckCanShuttle()
        {
            var rb = Player.Rb;
            var config = Player.Config;
            var state = Player.CheckState(EPlayerState.Air) 
                        || Player.CheckState(EPlayerState.Shuttle) 
                        || Player.CheckState(EPlayerState.Smash);
            if((state && rb.velocity.sqrMagnitude >= config.shuttleThreshold * config.shuttleThreshold) || _playerIn)
            {
                _canShuttle = true;
                UpdateState();
            }
            else if(_canShuttle)
            {
                _canShuttle = false;
                UpdateState();
            }
        }
    }
}