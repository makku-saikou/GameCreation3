using PurpleFlowerCore;

namespace PurpleTest
{
    public partial class TestUI : UINode
    {
        
    
        // Do not modify the region's name if you don't know how it works
        #region UI Event
        private void BtnButtonClick()
        {
            MegText.text = PFCConfig.WeaponData1.spear.ATK.ToString();
        }
        #endregion
    }
}
