using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePlay.Item
{
	public class FogOfWall : MonoBehaviour
	{
		[SerializeField] private List<SpriteRenderer> spriteRenderers;

		[SerializeField] [ReadOnly] private bool isFadeOut;

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.CompareTag("Player") && !isFadeOut && spriteRenderers.Count > 0)
			{
				foreach (var spriteRenderer in spriteRenderers)
					StartCoroutine(FogFadeOut(spriteRenderer));
			}
		}

		private IEnumerator FogFadeOut(SpriteRenderer spriteRenderer)
		{
			Color color = spriteRenderer.color;
			while (color.a > 0.05f)
			{
				color = Color.Lerp(color, Color.clear, Time.deltaTime);
				spriteRenderer.color = color;
				yield return null;
			}
			spriteRenderer.color = Color.clear;
			isFadeOut = true;
		}
	}
}
