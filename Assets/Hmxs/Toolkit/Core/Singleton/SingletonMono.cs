using UnityEngine;

namespace Hmxs.Toolkit
{
	/// <summary>
	/// generic singleton base class - inherit from MonoBehaviour
	/// </summary>
	public abstract class SingletonMono<T> : MonoBehaviour where T : SingletonMono<T>
	{
		private static T _instance;
		private static readonly object LockObj = new();
		private static bool _isApplicationQuitting;

		/// <summary>
		/// Global access point to the singleton instance.
		/// </summary>
		public static T Instance
		{
			get
			{
				if (_isApplicationQuitting)
				{
					Debug.LogWarning($"[{typeof(T)}] Instance is destroyed. Returning null.");
					return null;
				}

				lock (LockObj)
				{
					if (_instance) return _instance;
					_instance = FindObjectOfType<T>();
					if (_instance) return _instance;
					var singletonObject = new GameObject($"{typeof(T).Name}_SingletonMono");
					_instance = singletonObject.AddComponent<T>();
					return _instance;
				}
			}
		}

		/// <summary>
		/// Override this to control if the singleton survives scene changes.
		/// </summary>
		protected virtual bool KeepAliveAcrossScenes => true;

		protected virtual void Awake()
		{
			lock (LockObj)
			{
				if (!_instance)
				{
					_instance = this as T;
					if (KeepAliveAcrossScenes) DontDestroyOnLoad(gameObject);
				}
				else if (_instance != this)
				{
					Debug.LogWarning($"[{typeof(T)}] Duplicate instance detected. Destroying {name}.");
					DestroyImmediate(gameObject);
				}
			}
		}

		protected virtual void OnDestroy()
		{
			lock (LockObj) if (_instance == this) _instance = null;
		}

		protected virtual void OnApplicationQuit() => _isApplicationQuitting = true;
	}
}
