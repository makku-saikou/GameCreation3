using System.Collections.Generic;
using PurpleFlowerCore;

namespace Common.FSM
{
	public class LStateMachine : LState, IStateMachine 
	{
		private IState _currentState; // 当前状态
		private IState _defaultState; // 默认状态
		private List<IState> _states; // 所有状态

		private bool _isTransition;	// 是否在过渡
		private ITransition _t;	// 当前正在执行的过渡

		private List<ITransition> _anyStateTransitions; // 任何状态下的过渡
		
		public IState CurrentState => _currentState;
		
		public IState DefaultState 
		{
			get => _defaultState;
			set 
			{
				AddState (value); 
				_defaultState = value;
			}
		}
		
		public LStateMachine(string name, IState defaultState): base (name)
		{
			_states = new List<IState> ();
            _anyStateTransitions = new List<ITransition>();
			_defaultState = defaultState;
		}
		
		public void AddState(IState state)
		{
			if (state != null && !_states.Contains (state))
			{
				_states.Add (state);
				state.Parent = this;
				_defaultState ??= state;
			}
		}

		public void RemoveState (IState state)
		{
			// 状态机运行过程中,不能删除当前状态
			if (_currentState == state) 
			{
				PFCLog.Error("FSM","Can't remove current state");
				return;
			}
			if (state != null && _states.Contains (state)) 
			{
				_states.Remove (state);	
				state.Parent = null;
				if (_defaultState == state) 
				{
					_defaultState = _states.Count >= 1 ? _states [0] : null;
				}
			}
		}
		
		public IState GetStateWithTag (string tag)
		{
			return null;
		}
		
		public override void EnterCallback (IState prev)
		{
			base.EnterCallback (prev);
			_currentState.EnterCallback (prev);
		}
		
		public override void ExitCallback (IState next)
		{
			base.ExitCallback (next);
			_currentState.ExitCallback (next);
		}
		
		public override void UpdateCallback (float deltaTime)
		{
			if (_isTransition) 
			{
				if (_t.TransitionCallback()) 
				{
					DoTransition (_t);
					_isTransition = false;
				}
				return;
            }

            base.UpdateCallback (deltaTime);

            int count = _anyStateTransitions.Count;

            _currentState ??= _defaultState;

            // 首先检查任何状态下的过渡
            for (int i = 0; i < count; i++)
            {
                ITransition t = _anyStateTransitions[i];
                if (t.To != _currentState && t.ShouldBegin())
                {
                    _isTransition = true;
                    _t = t;
                    return;
                }
            }

            // 然后检查当前状态下的过渡
			List<ITransition> ts = _currentState.Transitions;
			count = ts.Count;
			for (int i = 0; i < count; i++) 
			{
				ITransition t = ts [i];
				if (t.ShouldBegin()) 
				{
					_isTransition = true;
					_t = t;
					return;
				}
			}
			_currentState.UpdateCallback (deltaTime);
		}
		
		public override void LateUpdateCallback (float deltaTime)
		{
			if (_isTransition) 
			{
				if (_t.TransitionCallback()) 
				{
					DoTransition (_t);
					_isTransition = false;
				}
				return;
			}
            base.LateUpdateCallback (deltaTime);

            _currentState ??= _defaultState;

            int count = _anyStateTransitions.Count;
            for (int i = 0; i < count; i++)
            {
                ITransition t = _anyStateTransitions [i];
                if (t.To!= _currentState && t.ShouldBegin())
                {
                    _isTransition = true;
                    _t = t;
                    return;
                }
            }
			List<ITransition> ts = _currentState.Transitions;
			count = ts.Count;
			for (int i = 0; i < count; i++)
			{
				ITransition t = ts [i];
				if (t.ShouldBegin()) 
				{
					_isTransition = true;
					_t = t;
					return;
				}
			}
			_currentState.LateUpdateCallback (deltaTime);
		}
		
		public override void FixedUpdateCallback ()
		{
			if (_isTransition) 
			{
				if (_t.TransitionCallback()) 
				{
					DoTransition (_t);
					_isTransition = false;
				}
				return;
			}
            base.FixedUpdateCallback ();

            _currentState ??= _defaultState;

            int count = _anyStateTransitions.Count;
            for (int i = 0; i < count; i++)
            {
                ITransition t = _anyStateTransitions [i];
                if (t.To!= _currentState && t.ShouldBegin())
                {
                    _isTransition = true;
                    _t = t;
                    return;
                }
            }

			List<ITransition> ts = _currentState.Transitions;
			count = ts.Count;
			for (int i = 0; i < count; i++) 
			{
				ITransition t = ts [i];
				if (t.ShouldBegin()) 
				{
					_isTransition = true;
					_t = t;
					return;
				}
			}
			_currentState.FixedUpdateCallback ();
		}

		private IState _tempState;
		private void DoTransition(ITransition t)
		{
            _tempState = _currentState;
			_currentState.ExitCallback (t.To);
			_currentState = t.To;
            if (t.From != null)
            {
                _tempState = t.From;
            }
            _currentState.EnterCallback(_tempState);
		}

        public void AddAnyState(ITransition t)
        {
            if (_anyStateTransitions.Contains(t))
                return;
            t.From = null;
            _anyStateTransitions.Add(t);
        }
	}
}