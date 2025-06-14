using System;
using UnityEngine;

namespace UI
{
	public class InteractableHint : MonoBehaviour
	{
		[SerializeField] private GameObject hintObject;

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.CompareTag("Player")) hintObject.SetActive(true);
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if (other.CompareTag("Player")) hintObject.SetActive(false);
		}
	}
}
