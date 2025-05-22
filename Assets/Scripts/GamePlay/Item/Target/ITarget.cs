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
        /// <summary>
        /// 是否是可吸附的，如果目标需要辅助瞄准或是移动目标，则为true
        /// </summary>
        public bool IsAdsorb { get; }
        public Vector3 AdsorbPosition { get; }
        /// <summary>
        /// 连接时，右键交互的逻辑
        /// </summary>
        /// <returns>交互逻辑发生后是否使舌头缩回</returns>
        public bool Interact(PlayerController playerController);
        /// <summary>
        /// 连接时舌尖的父物体
        /// </summary>
        public Transform Root { get; }
    }
}