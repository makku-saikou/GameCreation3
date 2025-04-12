using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePlay.Item
{
	public class BreakablePlatform : MonoBehaviour
	{
		[SerializeField] private float breakDelay = 1;
		[SerializeField] private float recoverDelay = 3;
		[SerializeField] [ReadOnly] private bool isBroken;

		private SpriteRenderer _spriteRenderer;
		private Collider2D _collider;

		private void Start()
		{
			_spriteRenderer = GetComponent<SpriteRenderer>();
			_collider = GetComponent<Collider2D>();
		}

		private void OnCollisionEnter2D(Collision2D other)
		{
			if (other.gameObject.CompareTag("Player") && !isBroken)
			{
				isBroken = true;
				Invoke(nameof(Break), breakDelay);
				Invoke(nameof(Recover), breakDelay + recoverDelay);
			}
		}

		private void Recover()
		{
			_spriteRenderer.enabled = true;
			_collider.enabled = true;
			isBroken = false;
		}

		private void Break()
		{
			_spriteRenderer.enabled = false;
			_collider.enabled = false;
		}
	}
}
