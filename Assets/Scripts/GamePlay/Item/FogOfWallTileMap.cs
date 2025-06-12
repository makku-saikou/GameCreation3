using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GamePlay.Item
{
	[RequireComponent(typeof(Tilemap))]
	public class FogOfWallTileMap : MonoBehaviour
	{
		[SerializeField] private float fadeSpeed = 1f;
		[SerializeField] [ReadOnly] private bool isFadeOut;

		private Tilemap _map;
		private Tilemap Map => _map ? _map : _map = GetComponent<Tilemap>();

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.CompareTag("Player") && !isFadeOut && Map)
				StartCoroutine(FogFadeOut(Map));
		}

		private IEnumerator FogFadeOut(Tilemap map)
		{
			Color color = map.color;
			while (color.a > 0.5f)
			{
				color = Color.Lerp(color, Color.clear, Time.deltaTime * fadeSpeed);
				map.color = color;
				yield return null;
			}
			map.color = Color.clear;
			isFadeOut = true;
		}
	}
}
