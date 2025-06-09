// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_3_31
// Description:
// -------------------------------------------------

using System;
using GamePlay.Player;
using Hmxs.Toolkit;
using PurpleFlowerCore;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Common.Manager
{
    public class GameManager : PurpleFlowerCore.Utility.SingletonMono<GameManager>
    {
        [SerializeField] private PlayerController playerPrefab;
        [SerializeField] private Transform bornPoint;
        private PlayerController _player;
        public PlayerController Player => _player;

        [SerializeField] private Transform checkPoint;

        [SerializeField] [ReadOnly] private int collectionCount;

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

        protected void  Start()
        {
            base.Awake();
            checkPoint = bornPoint;
            PlayerReset(bornPoint.position);
        }

        public void PlayerDie()
        {
            // TODO: 播放死亡动画，禁用角色输入
            PlayerReset(CheckPoint.position);
        }

        public void PlayerToTmpCheckPoint()
        {
            Timer.Register(0.5f, () => PlayerReset(TmpCheckPoint.position));
        }

        private void PlayerReset(Vector3 position)
        {
            if (_player)
                Destroy(_player.gameObject);
            _player = Instantiate(playerPrefab, position, Quaternion.identity);
            _player.Init();
            EventSystem.EventTrigger("PlayerInit");
        }

        public void GetCollection()
        {
            collectionCount++;
        }
    }
}
