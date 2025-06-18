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
            DebugSystem.AddCommand("Player/Die", () =>
            {
                GameManager.Instance.PlayerDie();
            });
            DebugSystem.AddCommand("Scene/MainMenu", () =>
            {
                UIManager.Instance.FadeOut(() =>
                {
                    SceneSystem.LoadScene(0);
                });
            });
            DebugSystem.AddCommand("Scene/Level-0", () =>
            {
                UIManager.Instance.FadeOut(() =>
                {
                    SceneSystem.LoadScene(1);
                });
            });
            DebugSystem.AddCommand("Scene/Level-1-1", () =>
            {
                UIManager.Instance.FadeOut(() =>
                {
                    SceneSystem.LoadScene(2);
                });
            });
            DebugSystem.AddCommand("Scene/Level-1-2", () =>
            {
                UIManager.Instance.FadeOut(() =>
                {
                    SceneSystem.LoadScene(3);
                });
            });
            DebugSystem.AddCommand("Scene/Level-1-3", () =>
            {
                UIManager.Instance.FadeOut(() =>
                {
                    SceneSystem.LoadScene(4);
                });
            });
            DebugSystem.AddCommand("Scene/Level-1-4", () =>
            {
                UIManager.Instance.FadeOut(() =>
                {
                    SceneSystem.LoadScene(5);
                });
            });
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