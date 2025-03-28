using UnityEngine;

namespace GamePlay.Item
{
	public class Trampoline : MonoBehaviour
	{
		[SerializeField] private float bounceForce = 10f;

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.CompareTag("Player"))
			{
				var rb = other.GetComponent<Rigidbody2D>();
				if (rb)
				{
					rb.velocity = new Vector2(rb.velocity.x, bounceForce);
				}
			}
		}
	}
}
