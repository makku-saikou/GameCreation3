// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_05_24
// Description:
// -------------------------------------------------

using System;
using GamePlay.Player;
using PurpleFlowerCore.Utility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Common.Manager
{
    public class UIManager : SingletonMono<UIManager>
    {
        [SerializeField] private Image blackPanel;
        private PlayerController Player => GameManager.Instance.Player;

        private void Start()
        {
            blackPanel.enabled = true;
            FadeIn();
        }

        public void FadeIn(UnityAction callback = null, float duration = 1f)
        {
            if (blackPanel)
            {
                blackPanel.CrossFadeAlpha(0, duration, false);
            }
            DelayUtility.Delay(duration, callback);
        }
        
        public void FadeOut(UnityAction callback = null, float duration = 1f)
        {
            if (blackPanel)
            {
                blackPanel.CrossFadeAlpha(1, duration, false);
            }
            DelayUtility.Delay(duration, callback);
        }
    }
}