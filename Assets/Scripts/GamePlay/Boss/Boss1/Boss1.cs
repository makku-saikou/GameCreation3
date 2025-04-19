using Hmxs.Toolkit;
using UnityEngine;

namespace GamePlay.Boss.Boss1
{
	public class Boss1 : SingletonMono<Boss1>
	{
		protected override bool KeepAliveAcrossScenes => false;

		[SerializeField] private Animator anim;

		public Animator Anim
		{
			get
			{
				if (anim) return anim;
				Debug.LogWarning("[Boss1] Animator is not assigned.");
				return null;
			}
		}
	}
}
