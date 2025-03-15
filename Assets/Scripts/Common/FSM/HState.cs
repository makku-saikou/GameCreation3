using System.Collections.Generic;

namespace Common.FSM
{
	public abstract class HState 
	{
		protected string _name; // 状态名
		protected string _tag; // 状态标签
		protected HStateMachine _parent; //当前状态的状态机
		protected List<HTransition> _transitions; //状态过渡
		
		public string Name => _name;
		
		public string Tag 
		{
			get => _tag;
			set => _tag = value;
		}
		
		public HStateMachine Parent
		{
			get => _parent;
			set => _parent = value;
		}
		
		public List<HTransition> Transitions => _transitions;
		
		public HState(string name)
		{
			_name = name;
			_transitions = new List<HTransition>();
		}

		public HState()
		{
			_name = "";
			_transitions = new List<HTransition>();
		}
		
		public virtual void AddTransition(HTransition t)
		{
			if (t != null && !_transitions.Contains(t)) 
			{
				_transitions.Add (t);	
			}
		}

		public virtual void EnterCallback(HState prev)
		{
			
		}

		public virtual void ExitCallback(HState next)
		{
			
		}

		public virtual void UpdateCallback(float deltaTime)
		{
			
		}

		public virtual void LateUpdateCallback(float deltaTime)
		{
			
		}

		public virtual void FixedUpdateCallback()
		{
			
		}
	}
}
