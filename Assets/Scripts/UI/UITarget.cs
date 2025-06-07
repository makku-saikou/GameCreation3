// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_06_08
// Description:
// -------------------------------------------------

using Common.Manager;
using PurpleFlowerCore;

namespace UI
{
    public partial class UITarget : UINode
    {
        private void OnEnable()
        {
            EventSystem.AddEventListener("PlayerInit", Init);
        }

        private void Init()
        {
            // todo: 太有侵入性了
            GameManager.Instance.Player.Head.Tongue.targetImage = TargetImage;
        }
    
        // Do not modify the region's name if you don't know how it works
        #region UI Event
        #endregion
    }
}