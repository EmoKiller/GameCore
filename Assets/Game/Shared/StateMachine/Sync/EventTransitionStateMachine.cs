using System;
using System.Collections.Generic;
using UnityEngine;
namespace Game.Share.StateMachine
{
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

            var typedEvent = (TEvent)evt; 

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



}
