// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_06_08
// Description:
// -------------------------------------------------

using PurpleFlowerCore;
using UnityEngine;

namespace GamePlay.Player
{
    public abstract class MonoPlayerProxy : MonoBehaviour
    {
        protected virtual void OnEnable()
        {
            EventSystem.AddEventListener("PlayerInit",Init);
        }
        
        protected virtual void OnDisable()
        {
            EventSystem.RemoveEventListener("PlayerInit",Init);
        }

        protected abstract void Init();
    }
}