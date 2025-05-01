using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace GamePlay.Boss.Boss1.Skills
{
	[TaskCategory("Custom/Boss1")]
	public class RocketPunch : Action
	{
		[SerializeField] private SharedGameObject _rocketPunchPrefab;
		[SerializeField] private readonly SharedFloat _duration;

		private float _startTime;

		public override void OnStart()
		{
			if (!_rocketPunchPrefab.Value)
				Debug.LogError("RocketPunch prefab is not assigned.");

			var rocketPunch = Object.Instantiate(_rocketPunchPrefab.Value);
			rocketPunch.transform.position = transform.position;
			rocketPunch.transform.rotation = transform.rotation;
			_startTime = Time.time;
		}

		public override TaskStatus OnUpdate()
		{
			return Time.time - _startTime >= _duration.Value ? TaskStatus.Success : TaskStatus.Running;
		}
	}
}
