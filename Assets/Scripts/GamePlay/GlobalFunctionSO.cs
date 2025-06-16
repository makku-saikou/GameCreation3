using Common.Manager;
using UnityEngine;

namespace GamePlay
{
	[CreateAssetMenu(fileName = "GlobalFunctionSO", menuName = "ScriptableObjects/GlobalFunctionSO", order = 1)]
	public class GlobalFunctionSO : ScriptableObject
	{
		public void DisablePlayerInput()
		{
			var player = GameManager.Instance.Player;
			if (player) player.Input.CanInput = false;
		}

		public void EnablePlayerInput()
		{
			var player = GameManager.Instance.Player;
			if (player) player.Input.CanInput = true;
		}
	}
}
