using System;
using System.Collections.Generic;
using Infastructure.StateMachine.States;

namespace Infastructure.StateMachine
{
    public class StateMachine
    {
        private Dictionary<Type, IState> _states;
        private IState _activeState;

        public StateMachine()
        {
            _states = new Dictionary<Type, IState>
            {
                { typeof(MenuState), new MenuState() },
                { typeof(PlayState), new PlayState() },
                { typeof(WinState), new WinState() },
                { typeof(FreeSpinState), new FreeSpinState() }
            };
        }

        public void Enter<TState>() where TState : class, IState
        {
            var state = ChangeState<TState>();
            state.Enter();
        }
        
        public TState ChangeState<TState>() where TState : class , IState
        {
            _activeState?.Exit();
            var state = GetState<TState>();
            _activeState = state;
            return state;
        }

        private TState GetState<TState>() where TState : class, IState
        {
            return _states[typeof(TState)] as TState;
        }
    }
}