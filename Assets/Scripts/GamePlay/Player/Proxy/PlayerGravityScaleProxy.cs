// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_05_27
// Description:
// -------------------------------------------------

using System.Collections.Generic;
using PurpleFlowerCore;
using UnityEngine;

namespace GamePlay.Player
{
    struct GravityEffectData
    {
        public string ID;
        public float Scale;
        public float Timer;
        public int Priority;
        // 当timer大于10000时，表示永久生效
        public GravityEffectData(string id, float scale, float timer = 10000, int priority = 0)
        {
            ID = id;
            Scale = scale;
            Timer = timer;
            Priority = priority;
        }
    }
    public class PlayerGravityScaleProxy
    {
        private PlayerController _player;
        List<GravityEffectData> _gravityEffects = new();
        private Rigidbody2D Rb => _player.Rb;
        private PlayerConfig Config => _player.Config;
        public float GravityScale
        {
            get => Rb.gravityScale;
            set
            {
                if (value < 0)
                {
                    PFCLog.Error("PlayerGravityScale","Gravity scale cannot be negative.");
                    return;
                }
                Rb.gravityScale = value;
            }
        }

        public PlayerGravityScaleProxy(PlayerController player)
        {
            _player = player;
        }

        public void Update()
        {
            UpdateGravityScale();
        }
        
        private void UpdateGravityScale()
        {
            if (_gravityEffects.Count == 0)
            {
                GravityScale = Config.gravityScale;
                return;
            }
            
            _gravityEffects.Sort((a, b) => b.Priority.CompareTo(a.Priority));
            var highestPriorityEffect = _gravityEffects[0];
            GravityScale = highestPriorityEffect.Scale;

            for (int i = _gravityEffects.Count - 1; i >= 0; i--)
            {
                var effect = _gravityEffects[i];
                if (effect.Timer < 10000)
                    effect.Timer -= Time.deltaTime;
                if (effect.Timer <= 0)
                {
                    _gravityEffects.RemoveAt(i);
                }
                else
                {
                    _gravityEffects[i] = effect;
                }
            }
        }
        
        public void AddGravityEffect(string id, float scale, float timer = 10000, int priority = 0)
        {
            if (scale < 0)
            {
                PFCLog.Error("PlayerGravityScale","Gravity scale cannot be negative.");
                return;
            }
            RemoveGravityEffect(id);
            _gravityEffects.Add(new GravityEffectData(id, scale, timer, priority));
        }

        public void RemoveGravityEffect(string id)
        {
            for (int i = _gravityEffects.Count - 1; i >= 0; i--)
            {
                if (_gravityEffects[i].ID == id)
                {
                    _gravityEffects.RemoveAt(i);
                    break;
                }
            }
        }

        public void RemoveAllGravityEffect()
        {
            _gravityEffects.Clear();
            GravityScale = Config.gravityScale;
        }
    }
}