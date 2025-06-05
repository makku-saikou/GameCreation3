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
    public class TargetPoint : MonoBehaviour, ITarget
    {
        public bool IsAdsorb => true;
        public Vector3 AdsorbPosition => transform.position;
        public Transform Root => transform;

        public bool Interact(PlayerController playerController)
        {
            return false;
        }
    }
}