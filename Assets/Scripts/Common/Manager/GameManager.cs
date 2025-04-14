// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_3_31
// Description:
// -------------------------------------------------

using GamePlay.Player;
using Hmxs.Toolkit;
using UnityEngine;

namespace Common.Manager
{
    public class GameManager : PurpleFlowerCore.Utility.SingletonMono<GameManager>
    {
        [SerializeField] private PlayerController player;
        public PlayerController Player => player;

        [SerializeField] private Transform checkPoint;
        public Transform CheckPoint
        {
            get => checkPoint;
            set => checkPoint = value;
        }

        [SerializeField] private Transform tmpCheckPoint;
        public Transform TmpCheckPoint
        {
            get => tmpCheckPoint;
            set => tmpCheckPoint = value;
        }

        public void PlayerDie()
        {
            // TODO: 播放死亡动画，禁用角色输入
            Timer.Register(2f, () => PlayerToCheckPoint(checkPoint.position));
        }

        public void PlayerToTmpCheckPoint()
        {
            Timer.Register(0.5f, () => PlayerToCheckPoint(tmpCheckPoint.position));
        }

        private void PlayerToCheckPoint(Vector3 position)
        {
            player.transform.position = position;
            player.Rb.velocity = Vector2.zero;
            player.Rb.angularVelocity = 0;
        }
    }
}
