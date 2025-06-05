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
using UnityEngine;

namespace GamePlay.Item
{
    public class ColorBlock_Shuttle : MonoBehaviour
    {
        [SerializeField] private EPlayerColor color = EPlayerColor.None;
        [SerializeField] private Collider2D collider2D;
        [SerializeField] private SpriteRenderer spriteRenderer;
        private PlayerController Player => GameManager.Instance.Player;
        private bool _canShuttle;
        private bool _playerIn;
        private void Start()
        {
            GameManager.Instance.Player.Property.OnColorChanged += OnPlayerColorChanged;
            switch (color)
            {
                case EPlayerColor.None:
                    PFCLog.Warning("颜色块", "颜色块没有设置颜色");
                    spriteRenderer.color = Color.white;
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
        }

        private void OnPlayerBumpInThis(Collision2D _)
        {
            PFCLog.Debug("ColorBlock", "player bumped when shuttling");
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