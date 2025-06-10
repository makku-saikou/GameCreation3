using System;
using Common.Manager;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePlay.Item
{
	public class Door : MonoBehaviour
	{
		[SerializeField] [ReadOnly] private bool isOpen;

		private Animator _animator;

		private void Start()
		{
			_animator = GetComponent<Animator>();
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (isOpen) return;
			if (other.CompareTag("Player") && GameManager.Instance.IsKeyCollected)
			{
				_animator.Play("open");
			}
		}
	}
}
