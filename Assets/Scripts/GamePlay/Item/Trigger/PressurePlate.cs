using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePlay.Item.Trigger
{
	public class PressurePlate : MonoBehaviour
	{
		[SerializeField] private Color originColor;
		[SerializeField] private Color triggerColor;
		[SerializeField] private TriggerTarget triggerTarget;
		[SerializeField] [ReadOnly] private bool isTriggered;

		private SpriteRenderer _spriteRenderer;

		private void Start()
		{
			_spriteRenderer = GetComponent<SpriteRenderer>();
		}

		private void OnTriggerStay2D(Collider2D other)
		{
			isTriggered = other.CompareTag("Player");
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			isTriggered = false;
		}

		private void Update()
		{
			if (!triggerTarget)
			{
				Debug.LogWarning(gameObject.name + "do not have trigger target");
			}

			if (isTriggered)
			{
				_spriteRenderer.color = triggerColor;
				triggerTarget.Trigger();
			}
			else
			{
				_spriteRenderer.color = originColor;
				triggerTarget.UnTrigger();
			}
		}
	}
}
