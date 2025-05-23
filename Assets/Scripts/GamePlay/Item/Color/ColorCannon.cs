using System;
using GamePlay.Player;
using Hmxs.Toolkit;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePlay.Item
{
	public class ColorCannon : MonoBehaviour
	{
		[SerializeField] private EPlayerColor color = EPlayerColor.Green;
		[SerializeField] [Range(0f, 360f)] private float direction;
		[SerializeField] private float force = 1f;
		[SerializeField] private Vector2 xyMaxSpeed = new Vector2(50, 50);
		[SerializeField] private float ejectDelay = 0.5f;
		[SerializeField] private float cooldown = 2f;

		private bool _isCooldown;

		private Vector2 FinalForce => Quaternion.Euler(0, 0, direction) * transform.rotation * Vector2.right * force * 100;

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (_isCooldown) return;
			if (!other.CompareTag("Player")) return;
			if (!other.TryGetComponent(out PlayerController player)) return;
			player.Property.IsInCannon = true;
			// todo: cannon eject animation
			Timer.Register(ejectDelay, () => EjectPlayer(player));
		}

		private void EjectPlayer(PlayerController player)
		{
			player.Property.CurrentColor = color;
			player.Property.XMaxSpeed = xyMaxSpeed.x;
			player.Property.YMaxSpeed = xyMaxSpeed.y;
			player.Rb.AddForce(FinalForce, ForceMode2D.Impulse);
			_isCooldown = true;
			player.Property.IsInCannon = false;
			// todo: cannon reload animation
			Timer.Register(cooldown, () => _isCooldown = false);
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawRay(transform.position, FinalForce / 50);
			Gizmos.DrawWireSphere(transform.position + (Vector3)FinalForce / 50, 0.5f);
		}
	}
}
