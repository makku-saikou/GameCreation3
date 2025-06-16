// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_06_16
// Description:
// -------------------------------------------------

using Common.Manager;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePlay.Item.Effect
{
    public class SceneSound : MonoBehaviour
    {
        [SerializeField] [LabelText("音效")] private AudioClip sound;
        [SerializeField] [LabelText("音量")] [Range(0f, 1f)]
        private float volume = 0.5f;
        [SerializeField] [LabelText("距离消散")] [Range(0f, 1f)] [Tooltip("越小，声音受距离的影响越小")]
        private float distanceFade = 1f;

        private void Start()
        {
            LoopPlay();
        }

        private void LoopPlay()
        {
            var source = AudioManager.PlayEffect(sound, transform, volume);
            source.loop = true;
            source.spatialBlend = distanceFade;
        }
    }
}