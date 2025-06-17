// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_06_10
// Description:
// -------------------------------------------------

using System;
using PurpleFlowerCore;
using PurpleFlowerCore.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Common.Manager
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private Image blackPanel;
        [SerializeField] private AudioClip bgm;

        private void Start()
        {
            AudioSystem.PlayBGM(bgm);

        }

        public void Play()
        {
            // FadeUtility.FadeInAndStay(blackPanel, 80, () =>
            // {
            //     SceneSystem.LoadScene(1);
            // });
            UIManager.Instance.FadeOut(() =>
            {
                SceneSystem.LoadScene(1);
            });
        }

        public void Exit()
        {
            Application.Quit();
        }
    }
}