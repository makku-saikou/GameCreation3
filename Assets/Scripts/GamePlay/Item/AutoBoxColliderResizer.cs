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
			if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
			if (!boxCollider) boxCollider = GetComponent<BoxCollider2D>();
			ColliderResize();
		}

		[Button]
		private void ColliderResize()
		{
			if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
			if (!boxCollider) boxCollider = GetComponent<BoxCollider2D>();

			bool note = false;
			if (!spriteRenderer)
			{
				Debug.LogError("SpriteRenderer not found!");
				note = true;
			}
			if (!boxCollider)
			{
				Debug.LogError("BoxCollider2D not found!");
				note = true;
			}
			if (note) return;

			var bounds = spriteRenderer.bounds;
			boxCollider.size = (Vector2)(bounds.size / spriteRenderer.transform.lossyScale.x) + sizeAdjust;
			boxCollider.offset = offset;
		}
	}
}
