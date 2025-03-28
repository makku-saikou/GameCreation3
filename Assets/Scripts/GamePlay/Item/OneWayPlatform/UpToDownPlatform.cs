using System;
using GamePlay.Player;
using UnityEngine;

namespace GamePlay.Item.OneWayPlatform
{
	public class UpToDownPlatform : MonoBehaviour
	{
		[SerializeField] private SmashMonitor smashMonitor;

		private PlatformEffector2D _platformEffector;

		private void Start() => _platformEffector = GetComponent<PlatformEffector2D>();

		private void Update()
		{
			_platformEffector.useOneWay = smashMonitor.IsSmashing;
		}
	}
}
