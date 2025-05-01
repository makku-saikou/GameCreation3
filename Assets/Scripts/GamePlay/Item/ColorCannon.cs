using System;
using GamePlay.Player;
using Hmxs.Toolkit;
using UnityEngine;

namespace GamePlay.Item
{
	public class ColorCannon : MonoBehaviour
	{
		[SerializeField] private EPlayerColor color = EPlayerColor.Green;
		[SerializeField] [Range(0f, 360f)] private float direction;
		[SerializeField] private float force = 10f;
		[SerializeField] private float ejectDelay = 0.5f;
		[SerializeField] private float cooldown = 2f;

		private bool _isCooldown;

		private Vector2 FinalForce => Quaternion.Euler(0, 0, direction) * transform.rotation * Vector2.right * force;

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (_isCooldown) return;
			if (!other.CompareTag("Player")) return;
			if (!other.TryGetComponent(out PlayerController player)) return;
			player.Property.IsInCannon = true;
			Timer.Register(ejectDelay, () => EjectPlayer(player));
		}

		private void EjectPlayer(PlayerController player)
		{
			player.Rb.velocity = Vector2.zero;
			player.Rb.AddForce(FinalForce, ForceMode2D.Impulse);
			_isCooldown = true;
			player.Property.IsInCannon = false;
			Timer.Register(cooldown, () => _isCooldown = false);
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawRay(transform.position, FinalForce);
			Gizmos.DrawWireSphere(transform.position + (Vector3)FinalForce, 0.5f);
		}
	}
}
