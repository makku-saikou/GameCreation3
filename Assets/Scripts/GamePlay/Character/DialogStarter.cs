
using System;
using Common.Manager;
using Fungus;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePlay.Character
{
	public class DialogStarter : MonoBehaviour
	{
		[SerializeField] private Flowchart flowchart;
		[SerializeField] private string dialogName;
		[SerializeField] [ReadOnly] private bool canTrigger;

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.CompareTag("Player")) canTrigger = true;
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if (other.CompareTag("Player")) canTrigger = false;
		}

		private void Update()
		{
			if (canTrigger && GameManager.Instance.Player.Input.InteractInput)
			{
				if (flowchart && !string.IsNullOrEmpty(dialogName))
				{
					flowchart.ExecuteBlock(dialogName);
				}
				else
					Debug.LogWarning("Flowchart or dialog name is not set in DialogStarter.");
			}
		}
	}
}
