using System.Collections.Generic;

namespace Common.FSM
{
	public delegate void LStateDelegate();
	public delegate void LStateDelegateState(IState state);
	public delegate void LStateDelegateFloat(float f);
	
	public class LState : IState 
	{
		private string _name; // 状态名
		private string _tag; // 状态标签
		private float _timer; // 计时器
		private IStateMachine _parent; //当前状态的状态机
		private List<ITransition> _transitions; //状态过渡
		
		public event LStateDelegateState OnEnter;
		
		public event LStateDelegateState OnExit;
		
		public event LStateDelegateFloat OnUpdate;
		
		public event LStateDelegateFloat OnLateUpdate;
		
		public event LStateDelegate OnFixedUpdate;
		
		public string Name => _name;
		
		public string Tag 
		{
			get => _tag;
			set => _tag = value;
		}
		
		public Common.FSM.IStateMachine Parent
		{
			get => _parent;
			set => _parent = value;
		}
		
		public float Timer => _timer;
		
		public List<Common.FSM.ITransition> Transitions => _transitions;
		
		public void AddTransition(Common.FSM.ITransition t)
		{
			if (t != null && !_transitions.Contains(t)) 
			{
				_transitions.Add (t);	
			}
		}
		
		public LState(string name)
		{
			_name = name;
			_transitions = new List<Common.FSM.ITransition>();
		}
		
		public virtual void EnterCallback(Common.FSM.IState prev)
		{
			_timer = 0f;
			OnEnter?.Invoke(prev);
		}
		
		public virtual void ExitCallback(Common.FSM.IState next)
		{
			_timer = 0f;
			OnExit?.Invoke (next);
		}
		
		public virtual void UpdateCallback (float deltaTime)
		{
			_timer += deltaTime;
			OnUpdate?.Invoke (deltaTime);
		}
		
		public virtual void LateUpdateCallback (float deltaTime)
		{
			OnLateUpdate?.Invoke (deltaTime);
		}
		
		public virtual void FixedUpdateCallback ()
		{
			OnFixedUpdate?.Invoke ();
		}
	}
}
