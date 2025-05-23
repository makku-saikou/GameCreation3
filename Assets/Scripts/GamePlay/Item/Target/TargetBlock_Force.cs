// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_05_24
// Description:
// -------------------------------------------------

using GamePlay.Player;
using PurpleFlowerCore.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePlay.Item.Target
{
    public class TargetBlock_Force : MonoBehaviour, ITarget
    {
        [SerializeField] [LabelText("拉力")] private float force = 200;
        [SerializeField] [LabelText("无重力时间")]private float noGravityTime = 0.5f;
        public bool IsAdsorb => false;
        public Vector3 AdsorbPosition { get; }
        public bool Interact(PlayerController playerController)
        {
            Vector3 direction = playerController.Entity.up;
            direction.Normalize();
            playerController.Rb.AddForce(direction * force, ForceMode2D.Impulse);
            // todo: 玩家数值系统的优化
            DelayUtility.DelayFrame(3, () =>
            {
                playerController.Rb.gravityScale = 0;
            });
            DelayUtility.Delay(noGravityTime, () =>
            {
                if(playerController.CurrentStateName == "Air" && playerController.Rb.gravityScale == 0)
                    playerController.Rb.gravityScale = playerController.Config.gravityScale;
            });
            return true;
        }

        public Transform Root => transform;
    }
}