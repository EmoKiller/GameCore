
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
namespace Game.Share.StateMachine
{
    #region CORE STATE MACHINE
    public class StateMachine<TStateId, TContext>
    {
        protected readonly struct StateCache
        {
            public readonly IState<TContext> State;
            public readonly IUpdateState<TContext> Update;
            public readonly IFixedUpdateState<TContext> FixedUpdate;

            public StateCache(IState<TContext> state)
            {
                State = state;
                Update = state as IUpdateState<TContext>;
                FixedUpdate = state as IFixedUpdateState<TContext>;
            }

        }
        
        protected readonly Dictionary<TStateId, StateCache> _states = new();
        protected readonly TContext _context;

        protected StateCache _current;

        public TStateId CurrentStateId { get; private set; }
        public IState<TContext> CurrentState => _current.State;
        
        

        public StateMachine(TContext context)
        {
            _context = context;
        }

        #region Registration

        public void RegisterState(TStateId id, IState<TContext> state)
        {
            _states[id] = new StateCache(state);
        }

        #endregion

        #region Change State

        public void ChangeState(TStateId stateId)
        {
            if (!_states.TryGetValue(stateId, out var newState))
                throw new Exception($"State not found: {stateId}");

            ChangeStateInternal(newState, stateId);
        }

        protected void ChangeStateInternal(StateCache newState, TStateId stateId)
        {
            _current.State?.Exit(_context);

            _current = newState;
            CurrentStateId = stateId;


            _current.State.Enter(_context);
        }

        #endregion

        #region Update Loops

        public virtual void Update(float deltaTime)
        {
            _current.Update?.Update(_context, deltaTime);
        }

        public void FixedUpdate(float fixedDeltaTime)
        {
            _current.FixedUpdate?.FixedUpdate(_context, fixedDeltaTime);
        }

        #endregion

    }
    #endregion
}


