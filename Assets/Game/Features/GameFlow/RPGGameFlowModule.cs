using System;
using Game.Application.Core;
using Game.Application.Core.SceneLoader;
using Game.Application.Events;
using Game.Application.Loading.Abstractions;
using Game.Application.Modules.Assets;
using Game.Application.Modules.UIModules;
using Game.Share.StateMachine;
using UnityEngine;

public class RPGGameFlowModule : BaseGameFlowModule<GameStateContext>
{
    public override string ModuleName => "RPGGameFlowModule";
    public override int InitializationOrder => 100;

    public override Type[] GetDependencies() => new[]
    {
        typeof(AssetModule),
        typeof(UIModule)
    };

    protected override GameStateContext CreateContext(IServiceContainer services)
    {
        return new GameStateContext(
            services.Resolve<ISceneLoader>(),
            services.Resolve<IUIService>(),
            services.Resolve<IEventBus>(),
            services.Resolve<IAssetProvider>(),
            services.Resolve<IPlayerService>(),
            services.Resolve<ICameraService>()
        );
    }

    protected override void RegisterStates(
        AsyncStateMachine<EGameState, GameStateContext> sm)
    {
        sm.RegisterState(EGameState.Boot, new BootState());
        sm.RegisterState(EGameState.MainMenu, new MainMenuState());
        sm.RegisterState(EGameState.Gameplay, new GameplayState());

        
    }
    protected override void RegisterLoadingState(
        AsyncStateMachine<EGameState, GameStateContext> sm)
    {
        sm.RegisterState(EGameState.Loading,
            new LoadingState<GameStateContext>(new ILoadingOperation<GameStateContext>[]
            {
                new MainMenuLoading(),
                new GamePlayLoading()
            }));
    } 
    protected override void RegisterTransitions(
        AsyncStateMachine<EGameState, GameStateContext> sm)
    {
        sm.AddTransition(new LoadingTransition<GameStateContext>());
    }

    protected override EGameState GetInitialState() => EGameState.Boot;

    protected override EGameState GetStartState() => EGameState.MainMenu;

    public override void Shutdown()
    {
        
    }
}
