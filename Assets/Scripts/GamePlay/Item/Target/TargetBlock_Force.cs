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
        // [SerializeField] [LabelText("无重力时间")]private float noGravityTime = 0.5f;
        public bool IsAdsorb => false;
        public Vector3 AdsorbPosition { get; }
        public bool Interact(PlayerController playerController)
        {
            Vector3 direction = Vector3.up;
            if (playerController.CheckState(EPlayerState.OnGround))
            {
                direction = playerController.Head.Tongue.transform.right;
            }
            else
            {
                direction = playerController.Entity.up;
            }
            direction.Normalize();
            var time = playerController.Config.interactNoGravityTime;
            playerController.Property.CanMove = false;
            DelayUtility.Delay(time, () =>
            {
                playerController.Property.CanMove = true;
            });
            playerController.Rb.AddForce(direction * force, ForceMode2D.Impulse);
            playerController.AddGravityEffect("TargetBlock_Force", 0,time);
            return true;
        }

        public Transform Root => transform;
    }
}