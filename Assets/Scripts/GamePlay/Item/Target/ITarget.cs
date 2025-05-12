// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_03_08
// Description:
// -------------------------------------------------

using GamePlay.Player;
using UnityEngine;

namespace GamePlay.Item.Target
{
    public interface ITarget
    {
        public bool IsAdsorb { get; }
        public Vector3 AdsorbPosition { get; }
        public void Interact(PlayerController playerController);
        public Transform Root { get; }
    }
}