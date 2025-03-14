
namespace Common.FSM
{
	public interface IStateMachine 
	{
		Common.FSM.IState CurrentState{ get; }
		
		Common.FSM.IState DefaultState{ get; set; }
		
		void AddState (Common.FSM.IState state);
		
		void RemoveState(Common.FSM.IState state);
		
		Common.FSM.IState GetStateWithTag(string tag);
		
        void AddAnyState(ITransition t);
	}
}
