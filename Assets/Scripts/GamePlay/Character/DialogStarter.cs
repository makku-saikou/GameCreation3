using System;
using Hmxs.Toolkit.Plugins.Fungus.FungusTools;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePlay.Character
{
	public class DialogStarter : MonoBehaviour
	{
		[SerializeField] private string dialogName;
		[SerializeField] [ReadOnly] private bool isTriggered;

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (isTriggered) return;
			if (other.CompareTag("Player"))
			{
				isTriggered = true;
				FlowchartManager.ExecuteBlock(dialogName);
			}
		}
	}
}
