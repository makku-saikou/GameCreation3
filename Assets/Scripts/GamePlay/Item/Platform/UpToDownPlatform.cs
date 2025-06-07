using System;
using GamePlay.Player;
using UnityEngine;

namespace GamePlay.Item.Platform
{
	public class UpToDownPlatform : MonoBehaviour
	{
		[SerializeField] private SmashMonitor smashMonitor;
		[SerializeField] private float openTime = 0.5f;

		private PlatformEffector2D _platformEffector;
		private float _openTimeCounter;

		private void Start() => _platformEffector = GetComponent<PlatformEffector2D>();

		private void Update()
		{
			if (_openTimeCounter > 0)
			{
				_openTimeCounter -= Time.deltaTime;
				_platformEffector.useOneWay = true;
				gameObject.layer = LayerMask.NameToLayer("Default");
			}
			else
			{
				_platformEffector.useOneWay = false;
				gameObject.layer = LayerMask.NameToLayer("Ground");
			}

			if (smashMonitor.IsSmashing) _openTimeCounter = openTime;
		}
	}
}
