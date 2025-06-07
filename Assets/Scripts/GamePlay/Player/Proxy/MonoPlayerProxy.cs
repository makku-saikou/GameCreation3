// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_06_08
// Description:
// -------------------------------------------------

using System;
using UnityEngine;

namespace GamePlay.Player
{
    [Obsolete]
    public abstract class MonoPlayerProxy : MonoBehaviour
    {
        // protected virtual void OnEnable()
        // {
        //     EventSystem.AddEventListener("PlayerInit",Start);
        // }
        //
        // protected virtual void OnDisable()
        // {
        //     EventSystem.RemoveEventListener("PlayerInit",Start);
        // }

        protected abstract void Start();
    }
}