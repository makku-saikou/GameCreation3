using System;
using System.Collections.Generic;
using Hmxs.Toolkit;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePlay.Item
{
	public class AutoMove : MonoBehaviour
	{
		private enum LoopType
		{
			None,
			PingPong,
			Loop
		}
		[InfoBox("AutoMove脚本用于在指定的路径上自动移动目标物体。")]
		[Tooltip("移动途径点")] [SerializeField] private List<Transform> waypoints;
		[Tooltip("被移动的物体")] [SerializeField] private Transform targetObject;
		[Tooltip("速度")] [SerializeField] private float speed = 2f;
		[Tooltip("循环类型")] [SerializeField] private LoopType loopType = LoopType.Loop;
		[InfoBox("None: 不循环\nPingPong: 往返循环\nLoop: 循环(到达最后点后会朝起点移动)")]
		[Tooltip("一轮循环结束后等待的时间")] [SerializeField] [HideIf("loopType", LoopType.None)] private float waitTime = 1f;
		[Tooltip("是否反向移动")] [SerializeField] private bool reverse;
		[SerializeField] [ReadOnly] private bool isMoving;

		private int _currentWaypointIndex;

		private void Start()
		{
			_currentWaypointIndex = reverse ? waypoints.Count - 1 : 0;
			targetObject.position = waypoints[_currentWaypointIndex].position;
			isMoving = true;
			Next();
		}

		private void Update()
		{
			if (!isMoving || waypoints.Count == 0) return;

			targetObject.position = Vector3.MoveTowards(
				targetObject.position,
				waypoints[_currentWaypointIndex].position,
				Time.deltaTime * speed);
			if (Vector3.Distance(targetObject.position, waypoints[_currentWaypointIndex].position) < 0.01f)
				Next();
		}

		private void Next()
		{
			_currentWaypointIndex += reverse ? -1 : 1;
			if (_currentWaypointIndex < 0 || _currentWaypointIndex >= waypoints.Count)
			{
				if (loopType == LoopType.PingPong)
				{
					isMoving = false;
					reverse = !reverse;
					_currentWaypointIndex += reverse ? -2 : 2;
					Timer.Register(waitTime, () => isMoving = true);
				}
				else if (loopType == LoopType.Loop)
				{
					_currentWaypointIndex = reverse ? waypoints.Count - 1 : 0;
				}
				else
				{
					isMoving = false;
					_currentWaypointIndex = Mathf.Clamp(_currentWaypointIndex, 0, waypoints.Count - 1);
				}
			}

			if (loopType == LoopType.Loop && _currentWaypointIndex == (reverse ? waypoints.Count - 2 : 1))
			{
				isMoving = false;
				Timer.Register(waitTime, () => isMoving = true);
			}
		}

		private void OnDrawGizmos()
		{
			if (waypoints.Count == 0 && !targetObject) return;
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(targetObject.position, 0.2f);
			for (int i = 0; i < waypoints.Count; i++)
			{
				Gizmos.color = i == _currentWaypointIndex ? Color.red : Color.green;
				Gizmos.DrawSphere(waypoints[i].position, 0.2f);
				if (i < waypoints.Count - 1 && reverse)
					Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
				else if (i > 0 && !reverse) Gizmos.DrawLine(waypoints[i].position, waypoints[i - 1].position);
				if (loopType != LoopType.Loop) break;
				if (i == waypoints.Count - 1 && reverse)
					Gizmos.DrawLine(waypoints[i].position, waypoints[0].position);
				else if (i == 0 && !reverse)
					Gizmos.DrawLine(waypoints[i].position, waypoints[^1].position);
			}
		}
	}
}
