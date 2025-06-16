// -------------------------------------------------
// Copyright@ makku-saikou
// Author : jianhao li
// Date: 2025_06_10
// Description:
// -------------------------------------------------

using System;
using System.Collections.Generic;
using PurpleFlowerCore;
using PurpleFlowerCore.Base;
using UnityEngine;
using UnityEngine.Events;
using Common.Manager;

namespace Common.Manager
{
    public static class AudioManager
    {
        private static GameObject _root;
        private static GameObject Root
        {
            get
            {
                if (_root is not null) return _root;
                _root = new GameObject("Audio")
                {
                    transform = { parent = PFCManager.Instance.transform }
                };
                return _root;
            }
        }
        
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
                PFCLog.Error("AudioManager", $"Audio clip '{name}' not found.");
                throw new Exception();
            }
            return clip;
        }
        
        // #region BGM
        //
        // private static AudioBGMModule _bgmModule;
        //
        // public static AudioBGMModule BGMModule
        // {
        //     get
        //     {
        //         if (_bgmModule is not null) return _bgmModule;
        //         _bgmModule = Root.AddComponent<AudioBGMModule>();
        //         return _bgmModule;
        //     }
        // }
        //
        // public static float BGMVolume
        // {
        //     get => BGMModule.Volume;
        //     set => BGMModule.Volume = value;
        //     
        // }
        //
        // public static bool BGMMute
        // {
        //     get => BGMModule.Mute;
        //     set => BGMModule.Mute = value;
        // }
        //
        // public static void PlayBGM(AudioClip clip)
        // {
        //     BGMModule.PlayBGM(clip);
        // }
        //
        // public static void PauseBGM()
        // {
        //     BGMModule.Pause();
        // }
        //
        // public static void UnpauseBGM()
        // {
        //     BGMModule.Unpause();
        // }
        //
        // #endregion

        #region Effect
        private static AudioEffectModule _effectModule;

        public static AudioEffectModule EffectModule
        {
            get
            {
                if (_effectModule is not null) return _effectModule;
                _effectModule = Root.AddComponent<AudioEffectModule>();
                return _effectModule;
            }
        }
        
        private static float _effectVolume = 1;
        public static float EffectVolume
        {
            get => _effectVolume;
            set => _effectVolume = Mathf.Clamp(value, 0, 1);
        }
        
        public static AudioSource PlayEffect(AudioClip clip,Transform parent = null, float volume = 1,UnityAction finishCallBack = null)
        {
            return EffectModule.Play(clip,parent,_effectVolume * volume,finishCallBack);
        }
        
        public static AudioSource PlayEffect(AudioClip clip,Vector3 position = default, float volume = 1,UnityAction finishCallBack = null)
        {
            return EffectModule.Play(clip,position,_effectVolume * volume,finishCallBack);
        }

        public static AudioSource PlayEffect(string clipName,Transform parent = null, float volume = 1, UnityAction finishCallBack = null)
        {
            var clip = Get(clipName);
            if (clip == null)
            {
                PFCLog.Error("AudioManager", $"Audio clip '{clipName}' not found.");
                return null;
            }
            return EffectModule.Play(clip, parent, _effectVolume * volume, finishCallBack);
        }
        
        public static AudioSource PlayEffect(string clipName,Vector3 position = default, float volume = 1, UnityAction finishCallBack = null)
        {
            var clip = Get(clipName);
            if (clip == null)
            {
                PFCLog.Error("AudioManager", $"Audio clip '{clipName}' not found.");
                return null;
            }
            return EffectModule.Play(clip, position, _effectVolume * volume, finishCallBack);
        }
        
        #endregion
    }
}