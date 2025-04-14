using Common.Manager;
using UnityEngine;

namespace GamePlay.Item.CheckPoint
{
	public class TmpCheckPoint : MonoBehaviour
	{
		[SerializeField] private Transform checkPoint;

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (!other.CompareTag("Player")) return;
			GameManager.Instance.TmpCheckPoint = checkPoint;
		}
	}
}
