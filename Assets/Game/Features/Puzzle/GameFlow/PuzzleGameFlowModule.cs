using System;
using Game.Application.Core;
using Game.Application.Core.SceneLoader;
using Game.Application.Events;
using Game.Application.Loading.Abstractions;
using Game.Application.Modules.Assets;
using Game.Application.Modules.UIModules;
using Game.Share.StateMachine;
using UnityEngine;

public class PuzzleGameFlowModule : BaseGameFlowModule<PuzzleGameStateContext>
{
    public override string ModuleName => "PuzzelGameFlowModule";
    public override int InitializationOrder => 100;

    public override Type[] GetDependencies() => new[]
    {
        typeof(AssetModule),
        typeof(UIModule)
    };

    protected override PuzzleGameStateContext CreateContext(IServiceContainer services)
    {
        return new PuzzleGameStateContext(
            services.Resolve<ISceneLoader>(),
            services.Resolve<IUIService>(),
            services.Resolve<IEventBus>(),
            services.Resolve<IAssetProvider>()
        );
    }

    protected override void RegisterStates(
        AsyncStateMachine<EGameState, PuzzleGameStateContext> sm)
    {
        sm.RegisterState(EGameState.Boot, new PuzzleBootState());
        sm.RegisterState(EGameState.MainMenu, new PuzzleMainMenuState());
        sm.RegisterState(EGameState.Gameplay, new PuzzleGameplayState());

        
    }
    protected override void RegisterLoadingState(
        AsyncStateMachine<EGameState, PuzzleGameStateContext> sm)
    {
        sm.RegisterState(EGameState.Loading,
            new LoadingState<PuzzleGameStateContext>(new ILoadingOperation<PuzzleGameStateContext>[]
            {
                new PuzzleMainMenuLoading(),
                new PuzzleGamePlayLoading()
            }));
    } 
    protected override void RegisterTransitions(
        AsyncStateMachine<EGameState, PuzzleGameStateContext> sm)
    {
        sm.AddTransition(new LoadingTransition<PuzzleGameStateContext>());
    }

    protected override EGameState GetInitialState() => EGameState.Boot;

    protected override EGameState GetStartState() => EGameState.MainMenu;

    public override void Shutdown()
    {
        
    }
}
