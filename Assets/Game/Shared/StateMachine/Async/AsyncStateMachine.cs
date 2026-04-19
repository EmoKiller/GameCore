
using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Share.StateMachine;
namespace Game.Share.StateMachine
{
    public class AsyncStateMachine<TStateId, TContext>
    {
        private readonly struct StateCache
        {
            public readonly IAsyncState<TContext> State;

            public StateCache(IAsyncState<TContext> state)
            {
                State = state;
            }
        }

        private readonly Dictionary<TStateId, StateCache> _states = new();
        private readonly Dictionary<TStateId, List<IAsyncTransition<TStateId, TContext>>> _transitions = new();

        private readonly TContext _context;
        public TContext Context => _context;
        private StateCache _current;

        public TStateId CurrentStateId { get; private set; }
        public bool HasState { get; private set; }

        public AsyncStateMachine(TContext context)
        {
            _context = context;
        }

        #region Registration

        public void RegisterState(TStateId id, IAsyncState<TContext> state)
        {
            _states[id] = new StateCache(state);
        }

        public void AddTransition(IAsyncTransition<TStateId, TContext> transition)
        {
            if (!_transitions.TryGetValue(transition.From, out var list))
            {
                list = new List<IAsyncTransition<TStateId, TContext>>();
                _transitions[transition.From] = list;
            }

            list.Add(transition);
        }

        #endregion

        #region State Control (CORE)

        public async UniTask SetInitialStateAsync(TStateId stateId, CancellationToken ct = default)
        {
            if (!_states.TryGetValue(stateId, out var state))
                throw new Exception($"State not found: {stateId}");

            _current = state;
            CurrentStateId = stateId;
            HasState = true;

            await _current.State.EnterAsync(_context, ct);
        }

        public async UniTask ChangeStateAsync(TStateId nextStateId, CancellationToken ct = default)
        {
            if (!HasState)
                throw new Exception("StateMachine not initialized. Call SetInitialStateAsync first.");

            if (!_states.TryGetValue(nextStateId, out var nextState))
                throw new Exception($"State not found: {nextStateId}");

            await _current.State.ExitAsync(_context, ct);

            _current = nextState;
            CurrentStateId = nextStateId;

            await _current.State.EnterAsync(_context, ct);
        }

        #endregion

        #region Transition Evaluation (OPTIONAL)

        public async UniTask<bool> TryTransitionAsync(CancellationToken ct = default)
        {
            if (!_transitions.TryGetValue(CurrentStateId, out var list))
                return false;

            for (int i = 0; i < list.Count; i++)
            {
                var transition = list[i];

                if (!await transition.CanTransitionAsync(_context, ct))
                    continue;
                    
                await ChangeStateAsync(transition.GetNextState(_context), ct);
                return true;
            }

            return false;
        }

        #endregion

    }
}
