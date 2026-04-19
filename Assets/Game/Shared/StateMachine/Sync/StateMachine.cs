
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
    #region Transition StateMachine    
    public class TransitionStateMachine<TStateId, TContext> : StateMachine<TStateId, TContext>
    {
        private readonly Dictionary<TStateId, List<ITransition<TStateId, TContext>>> _transitions 
            = new();

        public TransitionStateMachine(TContext context) : base(context)
        {
        }

        #region Registration

        public void AddTransition(ITransition<TStateId, TContext> transition)
        {
            if (!_transitions.TryGetValue(transition.From, out var list))
            {
                list = new List<ITransition<TStateId, TContext>>();
                _transitions[transition.From] = list;
            }

            list.Add(transition);
        }

        #endregion

        #region Trigger-based Evaluation

        /// <summary>
        /// Gọi khi có event xảy ra
        /// </summary>
        public bool TryTriggerTransition()
        {
            if (!_transitions.TryGetValue(CurrentStateId, out var list))
                return false;

            foreach (var transition in list)
            {
                if (!transition.CanTransition(_context))
                    continue;

                ChangeState(transition.To);
                return true;
            }

            return false;
        }

        #endregion
    }


    public class EventTransitionStateMachine<TStateId, TContext>
        : StateMachine<TStateId, TContext>
    {
        private readonly Dictionary<Type, ITransitionBucket<TStateId, TContext>> _buckets
            = new();

        public EventTransitionStateMachine(TContext context) : base(context)
        {
        }

        #region Registration

        public void AddTransition<TEvent>(
            IEventTransition<TStateId, TContext, TEvent> transition)
        {
            var eventType = typeof(TEvent);

            if (!_buckets.TryGetValue(eventType, out var bucket))
            {
                var newBucket = new TransitionBucket<TStateId, TContext, TEvent>();
                _buckets[eventType] = newBucket;
                bucket = newBucket;
            }

            ((TransitionBucket<TStateId, TContext, TEvent>)bucket)
                .Add(transition);
        }

        #endregion

        #region Trigger

        public bool TryTrigger<TEvent>(TEvent evt)
        {
            if (!_buckets.TryGetValue(typeof(TEvent), out var bucket))
                return false;

            var nextState = default(TStateId);

            if (!bucket.TryEvaluate(_context, CurrentStateId, ref nextState, evt))
                return false;

            ChangeState(nextState);
            return true;
        }

        #endregion
    }


    internal sealed class TransitionBucket<TStateId, TContext, TEvent> 
        : ITransitionBucket<TStateId, TContext>
    {
        private readonly Dictionary<TStateId, List<IEventTransition<TStateId, TContext, TEvent>>> _map;

        public TransitionBucket()
        {
            _map = new Dictionary<TStateId, List<IEventTransition<TStateId, TContext, TEvent>>>();
        }

        public void Add(IEventTransition<TStateId, TContext, TEvent> transition)
        {
            if (!_map.TryGetValue(transition.From, out var list))
            {
                list = new List<IEventTransition<TStateId, TContext, TEvent>>();
                _map[transition.From] = list;
            }

            list.Add(transition);
        }

        public bool TryEvaluate(TContext context, TStateId currentState, ref TStateId nextState, object evt)
        {
            if (!_map.TryGetValue(currentState, out var list))
                return false;

            var typedEvent = (TEvent)evt; // ⚠️ cast 1 lần duy nhất (no boxing)

            for (int i = 0; i < list.Count; i++)
            {
                var transition = list[i];

                if (!transition.CanTransition(context, typedEvent))
                    continue;

                nextState = transition.To;
                return true;
            }

            return false;
        }
    }









    #endregion
}


