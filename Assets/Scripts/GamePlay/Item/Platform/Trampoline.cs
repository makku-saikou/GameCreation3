using Common.Manager;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace GamePlay.Item.Platform
{
	public class Trampoline : MonoBehaviour
	{
		[SerializeField] private float bounceForce = 10f;
		public float BounceForce => bounceForce;

		[SerializeField] private MMF_Player feedbacks;

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.CompareTag("Player") && other.TryGetComponent(out Rigidbody2D rb))
			{
				rb.velocity = new Vector2(rb.velocity.x, bounceForce);
				feedbacks?.PlayFeedbacks();
				AudioManager.PlayEffect("蹦床触发声",transform.position);
			}
		}
	}
}
