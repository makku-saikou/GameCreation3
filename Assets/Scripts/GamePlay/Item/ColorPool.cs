using Common.Manager;
using GamePlay.Player;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePlay.Item
{
	public class ColorPool : MonoBehaviour
	{
		[SerializeField] private EPlayerColor color;

		[SerializeField] [ReadOnly] private bool isInPool;

		private void Update()
		{
			if (!isInPool) return;
			var player = GameManager.Instance.Player;
			if (!player)
			{
				Debug.LogWarning("[Color Pool] Play is NULL");
				return;
			}

			if (player.Input.InteractInput)
			{
				// 染色
				Debug.Log("Player Color Change: [ " + player.Property.CurrentColor + " ] -> [ " + color + " ]");
				player.Property.CurrentColor = color;
			}
		}

		private void OnTriggerStay2D(Collider2D other)
		{
			if (!other.CompareTag("Player")) return;
			isInPool = true;
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if (!other.CompareTag("Player")) return;
			isInPool = false;
		}
	}
}
