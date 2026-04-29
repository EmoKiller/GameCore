using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Game.Application.Core;
using Game.Application.Events;
using UnityEngine;

public class PuzzleModule : BaseGameModule
{
    public override string ModuleName => "PuzzleModule";

    public override int InitializationOrder => 10;

    public override Type[] GetDependencies() => Type.EmptyTypes;

    

    protected override async UniTask OnInitializeAsync(IServiceContainer services, CancellationToken ct)
    {
        var eventBus = services.Resolve<IEventBus>();    
        // ----------------------------------------
        // RANDOM
        // ----------------------------------------
        var randomProvider = new SystemRandomProvider();

        var randomTileProvider = new RandomTileProvider(new[]
                {
                    ETileType.Sword,
                    ETileType.Heart,
                    ETileType.Shield,
                    ETileType.Coin
                },
                randomProvider);
        // ----------------------------------------
        // SIMULATION
        // ----------------------------------------
        var swapSimulator = new SwapSimulator();

        // ----------------------------------------
        // MOVE
        // ----------------------------------------
        var moveFinder = new MoveFinder(swapSimulator);

        // ----------------------------------------
        // GENERATION
        // ----------------------------------------
        var boardGenerator = new SmartBoardGenerator(randomTileProvider,moveFinder);

        // ----------------------------------------
        // DETECTION
        // ----------------------------------------
        var matchDetector = new MatchDetector();
        var localMatchDetector = new LocalMatchDetector();
        

        // ----------------------------------------
        // RESOLUTION
        // ----------------------------------------
        var matchResolutionSystem = new MatchResolutionSystem(matchDetector,randomTileProvider);


        // ----------------------------------------
        // Combat
        // ----------------------------------------
        var combatContext = new NullCombatContext();
        // ----------------------------------------
        // EFFECT SYSTEM
        // ----------------------------------------
        var matchEffectRegistry = new MatchEffectRegistry(new Dictionary<ETileType, IMatchEffect>
                {
                    { ETileType.Sword, new SwordEffect() },
                    { ETileType.Heart, new HeartEffect() },
                    { ETileType.Shield, new ShieldEffect() },
                    { ETileType.Coin, new CoinEffect() }
                });

        var matchEffectProcessor = new MatchEffectProcessor(matchEffectRegistry);

        // ----------------------------------------
        // PUZZLE SYSTEM (FACADE)
        // ----------------------------------------
        var initializeBoardUseCase = new InitializeBoardUseCase(
            boardGenerator,
            eventBus
        );

        var swapUseCase = new SwapUseCase(
            swapSimulator,
            eventBus
        );

        var resolveMatchesUseCase = new ResolveMatchesUseCase(
            matchResolutionSystem,
            matchEffectProcessor,
            combatContext,
            eventBus
        );

        var ensurePlayableUseCase = new EnsurePlayableUseCase(
            moveFinder,
            boardGenerator,
            eventBus
        );


        services.Register<IPuzzleSystem>(c =>
            new PuzzleSystem(
                initializeBoardUseCase,
                swapUseCase,
                resolveMatchesUseCase,
                ensurePlayableUseCase
            ));

        await UniTask.CompletedTask;
    }
    public override void Shutdown()
    {
        
    }
}
