// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_06_10
// Description:
// -------------------------------------------------

using PurpleFlowerCore;
using UnityEngine;

namespace Common.Manager
{
    public class MainMenuManager : MonoBehaviour
    {
        public void Play()
        {
            SceneSystem.LoadScene(1);
        }

        public void Exit()
        {
            Application.Quit();
        }
    }
}