using UnityEngine;

namespace GamePlay.Item.Trigger
{
	public abstract class TriggerTarget : MonoBehaviour
	{
		public abstract void Trigger();

		public abstract void UnTrigger();
	}
}
