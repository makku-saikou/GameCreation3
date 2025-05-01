using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePlay.Item
{
	[RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer))]
	public class AutoBoxColliderResizer : MonoBehaviour
	{
		private SpriteRenderer _spriteRenderer;
		private BoxCollider2D _boxCollider;

		private void Start()
		{
			_spriteRenderer = GetComponent<SpriteRenderer>();
			_boxCollider = GetComponent<BoxCollider2D>();
			ColliderResize();
		}

		[Button]
		private void ColliderResize()
		{
			_spriteRenderer ??= GetComponent<SpriteRenderer>();
			_boxCollider ??= GetComponent<BoxCollider2D>();

			if (!_spriteRenderer || !_boxCollider)
			{
				Debug.LogError("SpriteRenderer or BoxCollider2D not found!");
				return;
			}

			var bounds = _spriteRenderer.bounds;
			_boxCollider.size = bounds.size / _spriteRenderer.transform.lossyScale.x;
		}
	}
}
