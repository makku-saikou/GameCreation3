// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_06_10
// Description:
// -------------------------------------------------

using System;
using System.Collections.Generic;
using PurpleFlowerCore;
using PurpleFlowerCore.Scripts.System.Audio;
using UnityEngine;

namespace Common.Manager
{
    // 暂时不要使用
    public static class AudioManager
    {
        // private static AudioSystemData _audioSystemData;
        // public static AudioSystemData AudioSystemData
        // {
        //     get
        //     {
        //         if (!_audioSystemData)
        //         {
        //             // _audioSystemData = GetSOByType(typeof (AudioSystemData)) as AudioSystemData;
        //             if (!_audioSystemData)
        //             {
        //                 PFCLog.Error("AudioManager","AudioSystemData not found in Resources folder.");
        //                 throw new Exception();
        //             }
        //         }
        //         return _audioSystemData;
        //     }
        // }

        private static string GetPath(string clip)
        {
            return $"Audio/{clip}";
        }

        private static AudioClip Get(string name)
        {
            // var clips = AudioSystemData.AudioClips;
            var clip = Resources.Load<AudioClip>(GetPath(name));
            // var theClip = clips.Find(clip => clip.name == name);
            if (!clip)
            {
                PFCLog.Error("AudioManager", $"Audio clip '{name}' not found in AudioSystemData.");
                throw new Exception();
            }
            return clip;
        }
        
        public static void PlayEffect(string audioName, Vector3 position)
        {
            var clip = Get(audioName);
            AudioSystem.PlayEffect(clip, position);
        }
    }
}