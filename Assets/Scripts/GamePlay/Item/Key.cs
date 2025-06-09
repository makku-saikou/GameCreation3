using System;
using Common.Manager;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePlay.Item
{
	public class Key : MonoBehaviour
	{
		private enum KeyState
		{
			Idle,
			Following
		}

		[Title("Idle Settings")]
		[SerializeField] private Vector2 idleOffset;
		[SerializeField] private float duration;
		[SerializeField] private Ease easeType = Ease.InOutSine;

		[Title("Follow Settings")]
		[SerializeField] private float followSpeed = 5f;
		[SerializeField] private float followingQuitThreshold = 2f;
		[SerializeField] private float idleInThreshold = 0.5f;

		[Title("Info")]
		[SerializeField] [ReadOnly] private bool isCollected;
		[SerializeField] [ReadOnly] private KeyState keyState = KeyState.Idle;
		[SerializeField] [ReadOnly] private bool isPlaying;

		private Tweener _tweener;
		private static Transform FollowPoint => GameManager.Instance.Player.KeyFollowPoint;

		private void Start() => StartIdle();

		private void Update()
		{
			switch (keyState)
			{
				case KeyState.Idle:
					IdleUpdate();
					break;
				case KeyState.Following:
					FollowingUpdate();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void FollowingUpdate()
		{
			if (Vector2.Distance(transform.position, FollowPoint.position) < idleInThreshold)
			{
				keyState = KeyState.Idle;
				return;
			}

			transform.position = Vector2.Lerp(transform.position, FollowPoint.position, followSpeed * Time.deltaTime);
		}

		private void IdleUpdate()
		{
			if (!isCollected) return;

			if (Vector2.Distance(transform.position, FollowPoint.position) > followingQuitThreshold)
			{
				keyState = KeyState.Following;
				StopIdle();
				return;
			}

			StartIdle();
		}

		private void StopIdle()
		{
			if (!isPlaying) return;
			_tweener.Kill();
			isPlaying = false;
		}

		private void StartIdle()
		{
			if (isPlaying) return;
			_tweener = transform.DOLocalMove(idleOffset, duration).SetEase(easeType).SetLoops(-1, LoopType.Yoyo).SetRelative();
			isPlaying = true;
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (isCollected) return;
			if (other.CompareTag("Player") || other.CompareTag("Tongue"))
			{
				isCollected = true;
				GameManager.Instance.GetKey();
			}
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(transform.position, followingQuitThreshold);
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, idleInThreshold);
		}
	}
}
