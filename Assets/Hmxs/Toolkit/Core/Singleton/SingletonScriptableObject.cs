using System;
using UnityEngine;

namespace Hmxs.Toolkit
{
    /// <summary>
    /// generic singleton base class - inherit from ScriptableObject
    /// </summary>
    public abstract class SingletonScriptableObject<T> : ScriptableObject where T : SingletonScriptableObject<T>
    {
        private static readonly Lazy<T> InstanceHolder = new(() =>
        {
            var instance = Resources.LoadAll<T>($"SingletonSO/{typeof(T)}");
            if (instance == null || instance.Length < 1)
                throw new Exception($"SingletonSO: Null {typeof(T)} SO instance in Resources directory");
            if (instance.Length > 1)
                Debug.LogWarning($"SingletonSO: Multiple {typeof(T)} SO instance in Resources directory");
            return instance[0];
        });
        
        public static T Instance => InstanceHolder.Value;
    }
}