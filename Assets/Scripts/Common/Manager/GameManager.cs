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

        [SerializeField] private Transform checkPoint;

        [Title("Game Info")]
        [SerializeField] [ReadOnly] private PlayerController player;
        [SerializeField] [ReadOnly] private int collectionCount;
        [SerializeField] [ReadOnly] private bool isKeyCollected;
        public PlayerController Player => player;
        public bool IsKeyCollected => isKeyCollected;

        public Transform CheckPoint
        {
            get => checkPoint;
            set => checkPoint = value;
        }

        // [SerializeField] private Transform tmpCheckPoint;
        // public Transform TmpCheckPoint
        // {
        //     get => tmpCheckPoint;
        //     set => tmpCheckPoint = value;
        // }

        protected void Start()
        {
#if UNITY_EDITOR
            var tempPlayer = FindObjectOfType<PlayerController>();
            if(tempPlayer)
                Destroy(tempPlayer.gameObject);
#endif
            checkPoint = bornPoint;
            PlayerReset(bornPoint.position);
        }

        public void PlayerDie()
        {
            // TODO: 播放死亡动画，禁用角色输入
            PlayerReset(CheckPoint.position);
            AudioManager.PlayEffect("玩家死亡音效",player.transform.position);
        }

        // public void PlayerToTmpCheckPoint()
        // {
        //     Timer.Register(0.5f, () => PlayerReset(TmpCheckPoint.position));
        // }

        private void PlayerReset(Vector3 position)
        {
            if (player)
                Destroy(player.gameObject);
            player = Instantiate(playerPrefab, position, Quaternion.identity);
            player.Init();
            EventSystem.EventTrigger("PlayerInit");
        }

        public void GetCollection()
        {
            collectionCount++;
        }

        public void GetKey()
        {
            isKeyCollected = true;
        }

        public void GameOver()
        {
            player.Property.CanMove = false;
            player.Property.CanFlip = false;
            player.Property.HeadCanMove = false;
            player.Rb.velocity = Vector3.zero;
        }
    }
}
