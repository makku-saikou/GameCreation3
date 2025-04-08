using GamePlay.Player;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePlay.Item.OneWayPlatform
{
	public class SmashMonitor : MonoBehaviour
	{
		public bool IsSmashing => isSmashing;

		[SerializeField] [ReadOnly] private bool isSmashing;

		private void OnTriggerStay2D(Collider2D other)
		{
			if (!other.gameObject.CompareTag("Player")) return;
			var player = other.gameObject.GetComponent<PlayerController>();
			if (player && player.CurrentStateName == "Smash")
				isSmashing = true;
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if (!other.gameObject.CompareTag("Player")) return;
			var player = other.gameObject.GetComponent<PlayerController>();
			if (player) isSmashing = false;
		}
	}
}
