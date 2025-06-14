// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_05_24
// Description:
// -------------------------------------------------

using System;
using GamePlay.Player;
using PurpleFlowerCore;
using PurpleFlowerCore.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Common.Manager
{
    public class UIManager : SingletonMono<UIManager>
    {
        [SerializeField] private Image blackPanel;

        [Title("World UI")]
        [SerializeField] private Canvas worldCanvas;
        [SerializeField] private Image timeCount;
        [SerializeField] private Vector2 timeCountOffset = new Vector2(1.5f, 1.5f);
        private GameObject TimeCountObj => timeCount ? timeCount.rectTransform.parent.gameObject : null;

        private static PlayerController Player => GameManager.Instance.Player;

        private void Start()
        {
            worldCanvas.worldCamera = Camera.main;
            blackPanel.enabled = true;
            FadeIn();
            // todo: 我知道这太复杂了
            EventSystem.AddEventListener("PlayerInit", () =>
            {
                Player.Property.OnCurrentColorDurationChanged += ((_, f) =>
                {
                    SetTimeCount(f / Player.Config.colorDuration);
                });
            });
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

        public void SetTimeCount(float percent)
        {
            if (timeCount)
            {
                timeCount.fillAmount = percent;
                SetTimeCountActive(percent > 0);
            }
        }

        public void SetTimeCountActive(bool active)
        {
            if (timeCount) TimeCountObj.SetActive(active);
        }

        private void Update()
        {
            if (TimeCountObj.activeSelf) TimeCountObj.transform.position = Player.TimeCountFollowPoint.position + (Vector3)timeCountOffset;
        }
    }
}
