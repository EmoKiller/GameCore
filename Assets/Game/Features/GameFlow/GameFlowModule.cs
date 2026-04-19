
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Application.Core.SceneLoader;
using Game.Application.Events;
using Game.Application.Loading.Abstractions;
using Game.Application.Modules.Assets;
using Game.Application.Modules.UIModules;
using Game.Share.StateMachine;
using UnityEngine;

namespace Game.Application.Core
{
    public sealed class GameFlowModule : BaseGameModule ,
        IEventHandler<RequestStateChangeEvent>
    {
        private AsyncStateMachine<EGameState,GameStateContext> _stateMachine;

        private CancellationTokenSource _stateCts;
        public override string ModuleName => "GameFlowModule";

        public override int InitializationOrder => 100;

        public int Priority => EventPriority.Normal;

        public EventChannel Channel => EventChannel.System;

        public override Type[] GetDependencies()
        {
            return new Type[]
            {
                typeof(AssetModule),
                typeof(UIModule)

            };
        }
        protected override async UniTask OnInitializeAsync(IServiceContainer services, CancellationToken ct)
        {
            services.Resolve<IEventBus>().Subscribe(this); 

            var context = new GameStateContext(
                services.Resolve<ISceneLoader>(),
                services.Resolve<IUIService>(),
                services.Resolve<IEventBus>(),
                services.Resolve<IAssetProvider>(),
                services.Resolve<IPlayerService>(),
                services.Resolve<ICameraService>()
            );     
            _stateMachine = new AsyncStateMachine<EGameState, GameStateContext>(context);

            //STATES
            _stateMachine.RegisterState(EGameState.Boot, new BootState());
            _stateMachine.RegisterState(EGameState.MainMenu, new MainMenuState());
            _stateMachine.RegisterState(EGameState.Gameplay, new GameplayState());

            //Loading State
            var loadingstate = new LoadingState(
                new ILoadingOperation[]
                {
                    new MainMenuLoading(),
                    new GamePlayLoading()

                }
            );

            _stateMachine.RegisterState(EGameState.Loading, loadingstate);

            // TRANSITIONS

            _stateMachine.AddTransition(new LoadingTransition());

            // Init defaut
            await _stateMachine.SetInitialStateAsync(EGameState.Boot, ct);

            // START
            await ChangeStateAsync(EGameState.MainMenu,ct); 

        }

        public async UniTask ChangeStateAsync(EGameState state, CancellationToken ct)
        {
            _stateMachine.Context.SetNextState(state);
            await _stateMachine.ChangeStateAsync(EGameState.Loading, ct);
            await _stateMachine.TryTransitionAsync(ct);
        }


        public override void Shutdown()
        {
            
        }

        public async void Handle(RequestStateChangeEvent evt)
        {
            Debug.Log("GameFlowModule handle");
            ResetStateCancellation();
            try
            {
                await ChangeStateAsync(evt.Target, _stateCts.Token);
            }
            catch (OperationCanceledException)
            {
                // expected when state is interrupted
            }
        }
        private void ResetStateCancellation()
        {
            _stateCts?.Cancel();
            _stateCts?.Dispose();
            _stateCts = new CancellationTokenSource();
        }
    }
}

