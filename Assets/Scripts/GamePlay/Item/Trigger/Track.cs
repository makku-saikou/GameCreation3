using System;
using UnityEngine;

namespace GamePlay.Item.Trigger
{
	public class Track : TriggerTarget
	{
		[SerializeField] private Transform target;
		[SerializeField] private Transform startPoint;
		[SerializeField] private Transform endPoint;
		[SerializeField] private float speed = 1f;

		public override void Trigger()
		{
			if (!startPoint || !endPoint || !target) return;
			target.position = Vector3.MoveTowards(target.position, endPoint.position, speed * Time.deltaTime);
		}

		public override void UnTrigger()
		{
			if (!startPoint || !endPoint || !target) return;
			target.position = Vector3.MoveTowards(target.position, startPoint.position, speed * Time.deltaTime);
		}

		private void OnDrawGizmos()
		{
			if (!startPoint || !endPoint) return;
			Gizmos.color = Color.red;
			Gizmos.DrawLine(startPoint.position, endPoint.position);
			Gizmos.DrawWireSphere(startPoint.position, 0.5f);
		}
	}
}
