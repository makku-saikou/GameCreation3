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
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Common.Manager
{
    public class UIManager : SingletonMono<UIManager>
    {
        [SerializeField] private FlowerPanel flowerPanel;
        [SerializeField] private int flowerX;
        [Title("World UI")]
        [SerializeField] private Canvas worldCanvas;
        [SerializeField] private Image timeCount;
        [SerializeField] private Vector2 timeCountOffset = new Vector2(1.5f, 1.5f);
        [SerializeField] private UITarget uiTarget;
        private GameObject TimeCountObj => timeCount ? timeCount.rectTransform.parent.gameObject : null;

        private static PlayerController Player => GameManager.Instance.Player;

        private void Start()
        {
            worldCanvas.worldCamera = Camera.main;
            flowerPanel.Enabled = true;
            FadeIn();
            // todo: 我知道这太复杂了
            // if (!GameManager.Instance) return;
            // DelayUtility.DelayFrame(1, () =>
            // {
            //
            // });
        }

        public void Init()
        {
            uiTarget.Init();
            Player.Property.OnCurrentColorDurationChanged += (_, f) =>
            {
                SetTimeCount(f / Player.Config.colorDuration);
            };
            EventSystem.AddEventListener("PlayerInit", () =>
            {
                Player.Property.OnCurrentColorDurationChanged += (_, f) =>
                {
                    SetTimeCount(f / Player.Config.colorDuration);
                };
            });
        }

        public void FadeIn(Action callback = null, float speed = 2000)
        {
            // if (flowerPanel)
            // {
            //     flowerPanel.CrossFadeAlpha(0, duration, false);
            // }
            flowerPanel.Move(0, -flowerX, callback, speed);
        }
        
        public void FadeOut(Action callback = null, float speed = 2000)
        {
            // if (flowerPanel)
            // {
            //     flowerPanel.CrossFadeAlpha(1, duration, false);
            // }
            // DelayUtility.Delay(duration, callback);
            flowerPanel.Move(flowerX, 0, callback, speed);
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
