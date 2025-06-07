// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_06_08
// Description:
// -------------------------------------------------

using Common.Manager;
using GamePlay.Player;
using PurpleFlowerCore;
using UnityEngine;

namespace GamePlay.Item
{
    public abstract class MapElement : MonoBehaviour
    {
        protected PlayerController Player => GameManager.Instance.Player;
        protected virtual void OnEnable()
        {
            EventSystem.AddEventListener("PlayerInit",Init);
        }
        
        protected virtual void OnDisable()
        {
            EventSystem.RemoveEventListener("PlayerInit",Init);
        }

        protected virtual void Init()
        {
            
        }
    }
}