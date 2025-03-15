namespace Common.FSM
{
	public delegate bool LTransitionDelegate();
	
	public class HTransition
	{
		private HState _from; // 原状态
		private HState _to;	// 目标状态
		private string _name; // 过渡名
		
		public event LTransitionDelegate OnTransition;
		public event LTransitionDelegate OnCheck;
		
		public HState From 
		{
			get => _from;
			set => _from = value;
		}
		
		public HState To 
		{
			get => _to;
			set => _to = value;
		}

		public string Name 
		{
			get => _name;
			set => _name = value;
		}
		
		public HTransition(string name,Common.FSM.HState fromState,Common.FSM.HState toState)
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
