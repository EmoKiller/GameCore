using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Game.Application.Core;
using Game.Application.Modules.Assets;
using Game.Application.UI.Core.Abstractions;
using Game.Presentation.UI;
using Game.Presentation.UI.Data;
using Game.Presentation.UI.View;
using UI.Core.Factory;
using UI.Core.ViewModel;
using UnityEngine;

namespace Game.Application.Modules.UIModules
{
    public class UIModule : BaseGameModule
    {
        public override string ModuleName => "UIModule";

        public override int InitializationOrder => 1;
        public override Type[] GetDependencies()
        {
            return new Type[]
            {
                typeof(AssetModule),

            };
        }
        protected override async UniTask OnInitializeAsync(IServiceContainer services, CancellationToken ct)
        {
            var assetProvider = services.Resolve<IAssetProvider>();
            var uiRoot = GameObject.FindFirstObjectByType<UIRoot>();
                
            // =========================
            // BUILD CONFIG
            // =========================
            var manifest = BuildManifest();
            var flowGraph = BuildFlowGraph();

            
            // =========================
            // Factory   (view + viewModel + viewPresenter)
            // =========================
            var view = new UIViewFactory(
                assetProvider,
                manifest,
                uiRoot
            );

            var viewModelFactory = new ViewModelFactory();

            var presenterFactory  = BuildPresenterFactory(manifest);


            // =========================
            // Debug
            // =========================
            var profiler = new UIProfilerService();
            var validator = new UIRuntimeValidator(false);
            

            // =========================
            // Pool
            // =========================
            var pool = new UIViewPool( view, profiler);  

            // =========================
            // BUILD CONFIG PreloadSystem
            // =========================
            var preload = BuildPreloadSystem(pool);   

            // =========================
            // 2. CORE SYSTEMS
            // =========================
            
            var composer = new UICompositionService(
                manifest,
                viewModelFactory,
                presenterFactory
            );

            var transition = new UITransitionSystem();
            //
            var presentation = new UIPresentationService(
                preload,
                pool,
                composer,
                transition,
                manifest,
                profiler,
                validator
                );


            var retentionCache = new UIRetentionCache(presentation,validator);

            var router = new UIRouter(
                presentation,
                manifest,
                retentionCache,
                profiler,
                validator);

            var navigator = new UINavigator(
                flowGraph,
                router,
                new UINavigationHistory());


            IUIService uiService = new UIService(navigator,router);

            // =========================
            // 3. REGISTER (CHỈ FACADE)
            // =========================
            
            services.Register(uiService);

            await UniTask.CompletedTask;

        }
        public override void Shutdown()
        {
            
        }

        public async UniTask ShowLoadingAsync(EGameState state)
        {
            Debug.Log(state  + "UImodule");
            await Task.CompletedTask;
        }

        private UIManifest BuildManifest()
        {

            var manifest = new UIManifest(new[]
            {
                new UIManifestEntry
                {
                    Id = "LoadingView",
                    AssetKey = "LoadingView",
                    ViewType = typeof(LoadingView),
                    ViewModelType = typeof(LoadingViewModel),
                    PresenterType = typeof(LoadingPresenter),
                    Layer = EUILayer.Overlay,
                    Lifetime = UILifetime.Overlay,
                    ReusePolicy = UIReusePolicy.Retain

                },

                new UIManifestEntry
                {
                    Id = "LoadingView",
                    AssetKey = "MainMenuScreen",
                    ViewType = typeof(MainMenuScreen),
                    ViewModelType = typeof(MainMenuScreenViewModel),
                    PresenterType = null,
                    Layer = EUILayer.Screen,
                    Lifetime = UILifetime.Screen,
                    ReusePolicy = UIReusePolicy.Cache
                },

                
            });

            

            return manifest;
        }
        private UIFlowGraph BuildFlowGraph()
        {
            var graph = new UIFlowGraph();

            var guardRegistry = new NavigationGuardRegistry();

            guardRegistry.Register("StartGame",
                new FuncNavigationGuard((from, to, ct) =>
                {
                    return UniTask.FromResult(true);
                }));

            guardRegistry.Register("Back",
                new FuncNavigationGuard((from, to, ct) =>
                {
                    return UniTask.FromResult(true);
                }));

            // =========================
            // GRAPH
            // =========================

            // graph.AddNode<MainMenuScreen>()
            //     .AddEdge<GameplayView>(
            //         "StartGame",
            //         guardRegistry.Resolve("StartGame"));

            // graph.AddNode<GameplayView>()
            //     .AddEdge<MainMenuScreen>(
            //         "Back",
            //         guardRegistry.Resolve("Back"));

            return graph;

        }

        private IUIPreloadSystem BuildPreloadSystem(UIViewPool pool)
        {
            var nodes = new IUIPreloadNode[]
            {
                new LoadingPreloadNode(),
                new MainMenuPreloadNode(),
            };

            var graph = new UIPreloadGraph(nodes);     



            return new UIPreloadSystem(graph, pool);
        }
        private PresenterFactory BuildPresenterFactory(UIManifest manifest)
        {
            var factory = new PresenterFactory();

            foreach (var entry in manifest.Entries)
            {
                if (entry.PresenterType == null)
                    continue;

                factory.RegisterDynamic(entry);
            }

            return factory;
        }
        
    }
}

