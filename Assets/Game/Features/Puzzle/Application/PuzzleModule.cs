using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Application.Core;
using UnityEngine;

public class PuzzleModule : BaseGameModule
{
    public override string ModuleName => "PuzzleModule";

    public override int InitializationOrder => 10;

    public override Type[] GetDependencies() => Type.EmptyTypes;

    public override void Shutdown()
    {
        
    }

    protected override async UniTask OnInitializeAsync(IServiceContainer services, CancellationToken ct)
    {
        var assetProvider = services.Resolve<IAssetProvider>();
        var handleSpecialResolverDatabase = await assetProvider.LoadAsync<ScriptableObject>("SpecialResolverDatabase", ct);
        var specialResolverDatabase = handleSpecialResolverDatabase.Asset as SpecialResolverDatabase;
        // Random
        var random = services.Resolve<IRandomProvider>();
        
        // Board
        var puzzleBoard = new PuzzleBoard(8,8);

        // SwapProcessor
        var matchResolver = new MatchResolver();

        var swapProcessor = new SwapProcessor(matchResolver); 

        // BoardGenerator
        var generator = new BoardGenerator(random);

        // CascadeProcessor

        var matchPatternAnalyzer = new MatchPatternAnalyzer();

        var specialTileResolver = new SpecialTileResolver(specialResolverDatabase);
        var specialTileProcessor = new SpecialTileProcessor(
            matchPatternAnalyzer,
            specialTileResolver
        );
        var SpecialActivationProcessor = new SpecialActivationProcessor();
        var specialActivationChainProcessor = new SpecialActivationChainProcessor(SpecialActivationProcessor);

        var removeMatchedTilesProcessor = new RemoveMatchedTilesProcessor();
        var gravityProcessor = new GravityProcessor();
        var spawnProcessor = new SpawnProcessor(random);

        var cascadeProcessor = new CascadeProcessor(
            matchResolver,
            specialTileProcessor,
            specialActivationChainProcessor,
            removeMatchedTilesProcessor,
            gravityProcessor,
            spawnProcessor
        ); 

        // PuzzleService
        var puzzleService = new PuzzleService(
            puzzleBoard,
            generator,
            swapProcessor,
            cascadeProcessor
        );  
        services.Register<IPuzzleService>(puzzleService);
    }
}
