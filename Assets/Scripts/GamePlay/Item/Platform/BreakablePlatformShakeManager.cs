using System;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePlay.Item.Platform
{
	public class BreakablePlatformShakeManager : MonoBehaviour
	{
		[Title("Position")]
		[SerializeField] private MMPositionShaker positionShaker;
		[SerializeField] [MinMaxSlider(0, 100f, true)] private Vector2 positionShakeSpeed;
		[SerializeField] [MinMaxSlider(0, 0.1f, true)] private Vector2 positionShakeRange;

		[Title("Rotation")]
		[SerializeField] private MMRotationShaker rotationShaker;
		[SerializeField] [MinMaxSlider(0, 100f, true)] private Vector2 rotationShakeSpeed;
		[SerializeField] [MinMaxSlider(0, 2f, true)] private Vector2 rotationShakeRange;

		public void Shake(float time)
		{
			if (!positionShaker.Shaking) positionShaker.Play();
			if (!rotationShaker.Shaking) rotationShaker.Play();
			positionShaker.ShakeSpeed = Mathf.Lerp(positionShakeSpeed.x, positionShakeSpeed.y, time);
			positionShaker.ShakeRange = Mathf.Lerp(positionShakeRange.x, positionShakeRange.y, time);
			rotationShaker.ShakeSpeed = Mathf.Lerp(rotationShakeSpeed.x, rotationShakeSpeed.y, time);
			rotationShaker.ShakeRange = Mathf.Lerp(rotationShakeRange.x, rotationShakeRange.y, time);
		}

		public void EndShake()
		{
			positionShaker.Stop();
			rotationShaker.Stop();
		}
	}
}
