// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_06_08
// Description:
// -------------------------------------------------

using Common.Manager;
using PurpleFlowerCore;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UITarget : MonoBehaviour
    {
        [SerializeField] private Image targetImage;
        private void OnEnable()
        {
            EventSystem.AddEventListener("PlayerInit", Init);
        }

        private void Init()
        {
            // todo: 太有侵入性了
            targetImage.enabled = true;
            GameManager.Instance.Player.Head.Tongue.targetImage = targetImage;
        }
    
        // Do not modify the region's name if you don't know how it works
        #region UI Event

        #endregion
    }
}