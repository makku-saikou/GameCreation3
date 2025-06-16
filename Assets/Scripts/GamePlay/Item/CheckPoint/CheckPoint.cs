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
		[SerializeField] private ParticleSystem particle;
		private bool _hasTriggered;
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (!other.CompareTag("Player")) return;

			if (GameManager.Instance.CheckPoint == checkPoint) return;
			GameManager.Instance.CheckPoint = checkPoint;
			// GameManager.Instance.TmpCheckPoint = checkPoint;
			// TODO: checkPoint animation
			flag.SetActive(true);
			flagFeedback?.PlayFeedbacks();
			if(!_hasTriggered)
			{
				AudioManager.PlayEffect("欢呼声",transform.position);
				particle.Play();
			}
			_hasTriggered = true;
		}
	}
}
