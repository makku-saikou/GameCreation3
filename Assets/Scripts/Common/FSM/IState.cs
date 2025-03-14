using System.Collections.Generic;

namespace Common.FSM
{
	public interface IState
	{
		string Name{get;}
		
		string Tag{ get; set; }
		
		IStateMachine Parent{get;set;}
		
		float Timer{get;}
		
		List<ITransition> Transitions{get;}
		
		void EnterCallback (IState prev);
		
		void ExitCallback (IState next);
		
		void UpdateCallback (float deltaTime);
		
		void LateUpdateCallback (float deltaTime);
		
		void FixedUpdateCallback ();
		
		void AddTransition (ITransition t);
	}
}
