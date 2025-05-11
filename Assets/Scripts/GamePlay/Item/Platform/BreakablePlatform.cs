using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePlay.Item.Platform
{
	public class BreakablePlatform : MonoBehaviour
	{
		[SerializeField] private float breakDelay = 1;
		[SerializeField] private float recoverDelay = 3;
		[SerializeField] [ReadOnly] private bool isBroken;

		[SerializeField] private SpriteRenderer spriteRenderer;
		[SerializeField] private Collider2D col;

		private void Start()
		{
			spriteRenderer ??= GetComponent<SpriteRenderer>();
			col ??= GetComponent<Collider2D>();
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (!other.CompareTag("Player") || isBroken) return;
			isBroken = true;
			Invoke(nameof(Break), breakDelay);
			Invoke(nameof(Recover), breakDelay + recoverDelay);
		}

		private void Recover()
		{
			spriteRenderer.enabled = true;
			col.enabled = true;
			isBroken = false;
		}

		private void Break()
		{
			spriteRenderer.enabled = false;
			col.enabled = false;
		}
	}
}
