using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;


namespace Game.Share.StateMachine
{
    internal struct StateTransitions<TStateId, TContext>
    {
        public TransitionGroup<TStateId, TContext> Interrupt;
        public TransitionGroup<TStateId, TContext> Main;
        public TransitionGroup<TStateId, TContext> Fallback;
    } 
    public enum TransitionPhase : byte
    {
        Interrupt = 0, // dead, hit
        Main = 1,      // combat, move
        Fallback = 2   // idle
    }
    public struct TransitionGroup<TStateId, TContext>
    {
        public ITransition<TStateId, TContext>[] Items;
        public int Count;
    }
    public interface ITransition<TStateId, TContext>
    {
        TStateId From { get; }
        TStateId To { get; }

        bool CanTransition(TContext context);
    }

    public interface IEventTransition<TStateId, TContext, TEvent>
    {
        TStateId From { get; }
        TStateId To { get; }

        bool CanTransition(TContext context, TEvent evt);
    }

    internal interface ITransitionBucket<TStateId, TContext>
    {
        bool TryEvaluate(TContext context, TStateId currentState, ref TStateId nextState, object evt);
    }


    public interface IAsyncTransition<TStateId, TContext>
    {
        TStateId From { get; }
        TStateId To { get; }

        UniTask<bool> CanTransitionAsync(TContext context, CancellationToken ct);
        TStateId GetNextState(TContext context);
    }

}