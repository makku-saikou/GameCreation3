// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_06_10
// Description:
// -------------------------------------------------

using PurpleFlowerCore;
using PurpleFlowerCore.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Common.Manager
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private Image blackPanel;
        public void Play()
        {
            FadeUtility.FadeInAndStay(blackPanel, 80, () =>
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