using System;
using System.Collections.Generic;

namespace Game.Share.StateMachine
{
    public class TransitionStateMachine<TStateId, TContext>
        : StateMachine<TStateId, TContext>
    {
        
        private readonly Dictionary<TStateId, StateTransitions<TStateId, TContext>> _map = new();

        public TransitionStateMachine(TContext context) : base(context)
        {
        }

        #region Registration

        public void AddTransition(
            ITransition<TStateId, TContext> transition,
            TransitionPhase phase = TransitionPhase.Main)
        {
            if (!_map.TryGetValue(transition.From, out var stateTransitions))
            {
                stateTransitions = new StateTransitions<TStateId, TContext>();
            }

            ref var group = ref GetGroup(ref stateTransitions, phase);

            AddToGroup(ref group, transition);

            _map[transition.From] = stateTransitions; // IMPORTANT
        }

        private static ref TransitionGroup<TStateId, TContext> GetGroup(
            ref StateTransitions<TStateId, TContext> transitions,
            TransitionPhase phase)
        {
            switch (phase)
            {
                case TransitionPhase.Interrupt:
                    return ref transitions.Interrupt;

                case TransitionPhase.Main:
                    return ref transitions.Main;

                default:
                    return ref transitions.Fallback;
            }
        }

        private static void AddToGroup(
            ref TransitionGroup<TStateId, TContext> group,
            ITransition<TStateId, TContext> transition)
        {
            if (group.Items == null)
            {
                group.Items = new ITransition<TStateId, TContext>[4];
            }
            else if (group.Count == group.Items.Length)
            {
                Array.Resize(ref group.Items, group.Items.Length * 2);
            }

            group.Items[group.Count++] = transition;
        }

        #endregion

        #region Evaluation

        public bool Evaluate()
        {
            if (!_map.TryGetValue(CurrentStateId, out var transitions))
                return false;

            if (EvaluateGroup(transitions.Interrupt))
                return true;

            if (EvaluateGroup(transitions.Main))
                return true;

            return EvaluateGroup(transitions.Fallback);
        }

        private bool EvaluateGroup(TransitionGroup<TStateId, TContext> group)
        {
            var items = group.Items;
            var count = group.Count;

            for (int i = 0; i < count; i++)
            {
                var t = items[i];

                if (!t.CanTransition(_context))
                    continue;

                ChangeState(t.To);
                return true; // EARLY EXIT
            }

            return false;
        }

        #endregion
    }
}
