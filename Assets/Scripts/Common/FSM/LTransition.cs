namespace Common.FSM
{
	public delegate bool LTransitionDelegate();
	
	public class LTransition : Common.FSM.ITransition 
	{
		private Common.FSM.IState _from; // 原状态
		private Common.FSM.IState _to;	// 目标状态
		private string _name; // 过渡名
		
		public event LTransitionDelegate OnTransition;
		public event LTransitionDelegate OnCheck;
		
		public Common.FSM.IState From 
		{
			get => _from;
			set => _from = value;
		}
		
		public Common.FSM.IState To 
		{
			get => _to;
			set => _to = value;
		}

		public string Name 
		{
			get => _name;
			set => _name = value;
		}
		
		public LTransition(string name,Common.FSM.IState fromState,Common.FSM.IState toState)
		{
			_name = name;
			_from = fromState;
			_to = toState;
		}
		
		public bool TransitionCallback()
		{
			return OnTransition == null || OnTransition();
		}

		public bool ShouldBegin ()
		{
			return OnCheck!=null && OnCheck ();
		}
	}
}
