// using System;
// using System.Collections.Generic;
// using Game.Share.StateMachine;
// using UnityEngine;

// public sealed class FuncTransition<TContext> : ITransition<TContext>
// {
//     private readonly Func<TContext, bool> _predicate;

//     public FuncTransition(Func<TContext, bool> predicate)
//     {
//         _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
//     }

//     public bool CanTransition(TContext context)
//     {
//         return _predicate.Invoke(context);
//     }
//     //_stateMachine.AddTransition( EGameState.Loading, EGameState.Gameplay, new FuncTransition<GameStateContext>(ctx => ctx.NextState == EGameState.Gameplay)
// }
// public sealed class AlwaysTrueTransition<TContext> : ITransition<TContext>
// {
//     public bool CanTransition(TContext context) => true;
// }

// public sealed class CompositeTransition<TContext> : ITransition<TContext>
// {
//     private readonly IReadOnlyList<ITransition<TContext>> _conditions;

//     public CompositeTransition(IReadOnlyList<ITransition<TContext>> conditions)
//     {
//         _conditions = conditions;
//     }

//     public bool CanTransition(TContext context)
//     {
//         foreach (var condition in _conditions)
//         {
//             if (!condition.CanTransition(context))
//                 return false;
//         }

//         return true;
//     }
//     // new CompositeTransition<GameStateContext>(new ITransition<GameStateContext>[]
//     // {
//     //     new FuncTransition<GameStateContext>(ctx => ctx.IsLoadingDone),
//     //     new FuncTransition<GameStateContext>(ctx => ctx.HasNetwork),
//     // })
// }
