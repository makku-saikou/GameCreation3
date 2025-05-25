using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePlay.Item.Platform
{
	public class DownToUpPlatform : MonoBehaviour
	{
		[SerializeField] [ReadOnly] private bool isPassing;

		private void OnTriggerStay2D(Collider2D other)
		{
			if (!other.CompareTag("Player")) return;
			isPassing = true;
		}

		private void OnCollisionExit2D(Collision2D other)
		{
			if (!other.collider.CompareTag("Player")) return;
			isPassing = false;
		}

		private void Update()
		{
			gameObject.layer = isPassing ? LayerMask.NameToLayer("Default") : LayerMask.NameToLayer("Ground");
		}
	}
}
