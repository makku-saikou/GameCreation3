// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_06_16
// Description:
// -------------------------------------------------

using Common.Manager;
using UnityEngine;

namespace GamePlay.Item.Effect
{
    public class SceneSound : MonoBehaviour
    {
        [SerializeField] private AudioClip sound;
        [SerializeField] private float volume = 0.5f;

        private void Start()
        {
            LoopPlay();
        }

        private void LoopPlay()
        {
            var source = AudioManager.PlayEffect(sound, transform, volume);
            source.loop = true;
            source.spatialBlend = 1f;
        }
    }
}