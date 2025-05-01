using System;
using GamePlay.Player;
using UnityEngine;

namespace GamePlay.Item
{
	public class ColorCannon : MonoBehaviour
	{
		[SerializeField] private EPlayerColor color = EPlayerColor.Green;
		[SerializeField] [Range(0f, 360f)] private float direction;
		[SerializeField] private float force = 10f;

		private Vector2 FinalForce => Quaternion.Euler(0, 0, direction) * transform.rotation * Vector2.right * force;



		private void Eject(GameObject target)
		{
			if (!target.TryGetComponent(out Rigidbody2D rb)) return;
			rb.velocity = Vector2.zero;
			rb.AddForce(FinalForce, ForceMode2D.Impulse);
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawRay(transform.position, FinalForce);
			Gizmos.DrawWireSphere(transform.position + (Vector3)FinalForce, 0.5f);
		}
	}
}
