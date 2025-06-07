using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Hmxs.Toolkit;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePlay.Item.Platform
{
	public class BreakablePlatform : MonoBehaviour
	{
		[SerializeField] private float breakDelay = 2;
		[SerializeField] private float breakDelayRecovery = 2f;
		[SerializeField] private float recoverDelay = 3;
		[SerializeField] [ReadOnly] private bool isBroken;
		[SerializeField] [ReadOnly] private BreakablePlatform left;
		[SerializeField] [ReadOnly] private BreakablePlatform right;

		[SerializeField] private SpriteRenderer spriteRenderer;
		[SerializeField] private BreakablePlatformShakeManager shakeManager;

		private BoxCollider2D _collider;
		private BoxCollider2D Collider => _collider ? _collider : GetComponents<BoxCollider2D>().First(col => !col.isTrigger);

		private BoxCollider2D _trigger;
		private BoxCollider2D Trigger => _trigger ? _trigger : GetComponents<BoxCollider2D>().First(col => col.isTrigger);

		private float _breakDelayRecoveryCount;
		private float _breakingCount;
		private bool _isBreaking;
		private bool _isTriggeredThisFrame;

		private void Start()
		{
			StartCoroutine(Init());
		}

		private IEnumerator Init()
		{
			yield return 0;
			yield return 0;
			left = Detect(Vector2.left);
			right = Detect(Vector2.right);
		}

		[Button]
		private BreakablePlatform Detect(Vector2 direction)
		{
			var bounds = Collider.bounds;
			var offset = new Vector3(direction.x * (bounds.extents.x + 0.1f), direction.y * (bounds.extents.y + 0.1f), 0);
			var hit = Physics2D.Raycast(transform.position + offset, direction, 0.5f);
			if (hit.collider && hit.collider.TryGetComponent(out BreakablePlatform platform) && platform != this)
				return platform;
			return null;
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			_isBreaking = other.CompareTag("Player") && !isBroken;
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			_isBreaking = false;
		}

		private void Update()
		{
			if (_isBreaking)
			{
				_breakingCount += Time.deltaTime;
				Breaking(_breakingCount);
				_breakDelayRecoveryCount = 0;
			}
			else
			{
				_breakDelayRecoveryCount += Time.deltaTime;
				if (_breakDelayRecoveryCount > breakDelayRecovery)
				{
					_breakDelayRecoveryCount = 0;
					_breakingCount = 0;
					shakeManager.EndShake();
				}
			}
		}

		private void LateUpdate()
		{
			_isTriggeredThisFrame = false;
		}

		private void Breaking(float breakingCount)
		{
			if (isBroken || _isTriggeredThisFrame) return;
			_isTriggeredThisFrame = true;
			_breakingCount = breakingCount;
			if (_breakingCount > breakDelay)
			{
				_breakingCount = 0;
				Break();
			}

			shakeManager.Shake(_breakingCount / breakDelay);

			if (left) left.Breaking(breakingCount);
			if (right) right.Breaking(breakingCount);
		}

		private void Break()
		{
			if (isBroken) return;
			Debug.Log(name + " is broken!");
			spriteRenderer.enabled = false;
			Collider.enabled = false;
			Trigger.enabled = false;
			isBroken = true;
			shakeManager.EndShake();
			Timer.Register(recoverDelay, Recover);

			if (left) left.Break();
			if (right) right.Break();
		}

		private void Recover()
		{
			if (!isBroken) return;
			Debug.Log(name + " is recovered!");
			spriteRenderer.enabled = true;
			Collider.enabled = true;
			Trigger.enabled = true;
			isBroken = false;
			_breakingCount = 0;

			if (left) left.Recover();
			if (right) right.Recover();
		}
	}
}
