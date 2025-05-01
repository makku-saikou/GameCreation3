using Common.FSM;
using PurpleFlowerCore;

namespace GamePlay.Player.PlayerState
{
	public class InCannonState : PlayerStateBase
	{
		public InCannonState(PlayerController player, string name) : base(player, name) { }

		public override void EnterCallback(HState prev)
		{
			base.EnterCallback(prev);
			PFCLog.Debug("Enter InCannon State");
			Player.Entity.gameObject.SetActive(false);
		}

		public override void ExitCallback(HState next)
		{
			base.ExitCallback(next);
			PFCLog.Debug("Exit InCannon State");
			Player.Entity.gameObject.SetActive(true);
		}
	}
}
