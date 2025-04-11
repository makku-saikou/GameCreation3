// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_3_31
// Description:
// -------------------------------------------------

using GamePlay.Player;
using PurpleFlowerCore.Utility;
using UnityEngine;

namespace Common.Manager
{
    public class GameManager : SingletonMono<GameManager>
    {
        [SerializeField] private PlayerController player;
        public PlayerController Player => player;
        
    }
}