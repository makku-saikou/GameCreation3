using System;
using HighlightPlus2D;
using UnityEngine;

namespace GamePlay.Item
{
	public class HighlightTrigger : MonoBehaviour
	{
		[SerializeField] private HighlightEffect2D highlight;

		private void OnTriggerStay2D(Collider2D other)
		{
			if (other.CompareTag("Player"))
				if (highlight) highlight.highlighted = true;
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if (other.CompareTag("Player"))
				if (highlight) highlight.highlighted = false;
		}
	}
}
