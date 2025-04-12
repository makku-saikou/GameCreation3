using System;
using UnityEngine;

namespace GamePlay.Item
{
	public class Thorn : MonoBehaviour
	{
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.CompareTag("Player"))
			{
				// player die
			}
		}
	}
}
