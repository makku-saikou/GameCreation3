// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_06_10
// Description:
// -------------------------------------------------

using Common.Manager;
using PurpleFlowerCore;
using UnityEngine;

namespace GamePlay.Item.CheckPoint
{
    public class NextLevel : MonoBehaviour
    {
        [SerializeField] private int nextLevel;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            GameManager.Instance.GameOver();
            UIManager.Instance.FadeOut(() =>
            {
                SceneSystem.LoadScene(nextLevel);
            });
        }
    }
}