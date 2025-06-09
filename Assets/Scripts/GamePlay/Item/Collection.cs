using System;
using Common.Manager;
using UnityEngine;

namespace GamePlay.Item
{
	public class Collection : MonoBehaviour
	{
		[SerializeField] private GameObject feedbackParticle;
		[SerializeField] private float particleDestroyDelay = 2f;

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
			{
				Collect();
			}
			// if (other.CompareTag("Tongue") || other.CompareTag("Player"))
			// {
			// 	Collect();
			// }
		}

		protected virtual void Collect()
		{
			GameManager.Instance.GetCollection();
			if (feedbackParticle)
			{
				var particle = Instantiate(feedbackParticle, transform.position, Quaternion.identity);
				Destroy(particle, particleDestroyDelay);
			}
			Destroy(gameObject);
		}
	}
}
