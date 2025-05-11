using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePlay.Item
{
	public class AutoBoxColliderResizer : MonoBehaviour
	{
		[SerializeField] private SpriteRenderer spriteRenderer;
		[SerializeField] private BoxCollider2D boxCollider;

		[SerializeField] [OnValueChanged("ColliderResize")] private Vector2 offset = Vector2.zero;
		[SerializeField] [OnValueChanged("ColliderResize")] private Vector2 sizeAdjust = Vector2.zero;

		private void Start()
		{
			spriteRenderer ??= GetComponent<SpriteRenderer>();
			boxCollider ??= GetComponent<BoxCollider2D>();
			ColliderResize();
		}

		[Button]
		private void ColliderResize()
		{
			spriteRenderer ??= GetComponent<SpriteRenderer>();
			boxCollider ??= GetComponent<BoxCollider2D>();

			if (!spriteRenderer || !boxCollider)
			{
				Debug.LogError("SpriteRenderer or BoxCollider2D not found!");
				return;
			}

			var bounds = spriteRenderer.bounds;
			boxCollider.size = (Vector2)(bounds.size / spriteRenderer.transform.lossyScale.x) + sizeAdjust;
			boxCollider.offset = offset;
		}
	}
}
