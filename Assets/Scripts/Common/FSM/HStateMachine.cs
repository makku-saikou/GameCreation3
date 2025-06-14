using System;
using System.Collections.Generic;
using PurpleFlowerCore;

namespace Common.FSM
{
	// 注意这里的状态机和PFC的状态机名称类似，在这个项目里我们暂时使用Common。FSM的状态机
	public class HStateMachine : HState
	{
		public override string Name => "HStateMachine";
		private HState _currentState; // 当前状态
		private HState _defaultState; // 默认状态
		private List<HState> _states; // 所有状态

		private bool _isTransition;	// 是否在过渡
		private HTransition _t;	// 当前正在执行的过渡

		private List<HTransition> _anyStateTransitions; // 任何状态下的过渡
		
		public HState CurrentState => _currentState;
		
		public event Action<HState, HState> OnStateChanged;
		
		public HState DefaultState 
		{
			get => _defaultState;
			set 
			{
				AddState (value); 
				_defaultState = value;
			}
		}
		
		public HStateMachine(HState defaultState)
		{
			_states = new List<HState> ();
            _anyStateTransitions = new List<HTransition>();
			_defaultState = defaultState;
			_currentState = defaultState;
		}
		
		public void AddState(HState state)
		{
			if (state != null && !_states.Contains (state))
			{
				_states.Add (state);
				state.Parent = this;
				_defaultState ??= state;
			}
		}

		public void RemoveState (HState state)
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
		
		public HState GetStateWithTag (string tag)
		{
			return null;
		}


		public override void EnterCallback (HState prev)
		{
			_currentState.EnterCallback (prev);
		}
		
		public override void ExitCallback (HState next)
		{
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
			
            int count = _anyStateTransitions.Count;

            _currentState ??= _defaultState;

            // 首先检查任何状态下的过渡
            for (int i = 0; i < count; i++)
            {
	            HTransition t = _anyStateTransitions[i];
                if (t.To != _currentState && t.ShouldBegin())
                {
                    _isTransition = true;
                    _t = t;
                    return;
                }
            }

            // 然后检查当前状态下的过渡
			List<HTransition> ts = _currentState.Transitions;
			count = ts.Count;
			for (int i = 0; i < count; i++) 
			{
				HTransition t = ts [i];
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

            _currentState ??= _defaultState;

            int count = _anyStateTransitions.Count;
            for (int i = 0; i < count; i++)
            {
	            HTransition t = _anyStateTransitions [i];
                if (t.To!= _currentState && t.ShouldBegin())
                {
                    _isTransition = true;
                    _t = t;
                    return;
                }
            }
			List<HTransition> ts = _currentState.Transitions;
			count = ts.Count;
			for (int i = 0; i < count; i++)
			{
				HTransition t = ts [i];
				if (t.ShouldBegin()) 
				{
					_isTransition = true;
					_t = t;
					return;
				}
			}
			_currentState.LateUpdateCallback(deltaTime);
		}
		
		public override void FixedUpdateCallback()
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

            _currentState ??= _defaultState;

            int count = _anyStateTransitions.Count;
            for (int i = 0; i < count; i++)
            {
	            HTransition t = _anyStateTransitions [i];
                if (t.To!= _currentState && t.ShouldBegin())
                {
                    _isTransition = true;
                    _t = t;
                    return;
                }
            }

			List<HTransition> ts = _currentState.Transitions;
			count = ts.Count;
			for (int i = 0; i < count; i++) 
			{
				HTransition t = ts [i];
				if (t.ShouldBegin()) 
				{
					_isTransition = true;
					_t = t;
					return;
				}
			}
			_currentState.FixedUpdateCallback ();
		}

		private HState _tempState;
		private void DoTransition(HTransition t)
		{
            _tempState = _currentState;
			_currentState.ExitCallback (t.To);
			_currentState = t.To;
            if (t.From != null)
            {
                _tempState = t.From;
            }
            OnStateChanged?.Invoke(_tempState, _currentState);
            _currentState.EnterCallback(_tempState);
		}

        public void AddAnyState(HTransition t)
        {
            if (_anyStateTransitions.Contains(t))
                return;
            t.From = null;
            _anyStateTransitions.Add(t);
        }
	}
}