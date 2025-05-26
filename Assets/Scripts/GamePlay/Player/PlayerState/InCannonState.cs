using Common.FSM;
using Hmxs.Toolkit;
using PurpleFlowerCore;
using UnityEngine;

namespace GamePlay.Player.PlayerState
{
	public class InCannonState : PlayerStateBase
	{
		public InCannonState(PlayerController player, string name) : base(player, name) { }

		// private float _gravityScale;

		public override void EnterCallback(HState prev)
		{
			base.EnterCallback(prev);
			PFCLog.Debug("Enter InCannon State");
			Player.Entity.gameObject.SetActive(false);
			Player.Rb.velocity = Vector2.zero;
			Player.Rb.angularVelocity = 0f;
			Player.Rb.rotation = 0f;
			// Player.Rb.gravityScale = 0;
			Player.Rb.constraints = RigidbodyConstraints2D.FreezeAll;
			// _gravityScale = Player.Rb.gravityScale;
		}

		public override void ExitCallback(HState next)
		{
			base.ExitCallback(next);
			PFCLog.Debug("Exit InCannon State");
			Player.Entity.gameObject.SetActive(true);
			// Timer.Register(1f, () =>
			// {
			// 	if (Player.Rb.gravityScale == 0)
			// 		Player.Rb.gravityScale = _gravityScale;
			// });
		}
	}
}
