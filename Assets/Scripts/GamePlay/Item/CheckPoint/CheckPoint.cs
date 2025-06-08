using System;
using Common.Manager;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace GamePlay.Item.CheckPoint
{
	public class CheckPoint : MonoBehaviour
	{
		[SerializeField] private Transform checkPoint;
		[SerializeField] private GameObject flag;
		[SerializeField] private MMF_Player flagFeedback;

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (!other.CompareTag("Player")) return;

			if (GameManager.Instance.CheckPoint == checkPoint) return;
			GameManager.Instance.CheckPoint = checkPoint;
			GameManager.Instance.TmpCheckPoint = checkPoint;
			// TODO: checkPoint animation
			flag.SetActive(true);
			flagFeedback?.PlayFeedbacks();
		}
	}
}
