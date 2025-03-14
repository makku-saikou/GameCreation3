namespace Common.FSM
{
	public interface ITransition 
	{
		Common.FSM.IState From{get;set;}
		
		Common.FSM.IState To{get;set;}
		
		string Name{ get; set; }
		
		bool TransitionCallback();
		
		bool ShouldBegin ();
	}
}
