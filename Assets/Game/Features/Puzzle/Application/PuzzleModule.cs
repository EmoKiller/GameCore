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
        // Random
        var random = services.Resolve<IRandomProvider>();
        
        // Board
        var puzzleBoard = new PuzzleBoard(8,8);

        // SwapProcessor
        var matchResolver = new MatchResolver();

        var swapProcessor = new SwapProcessor(
            matchResolver
        ); 

        // BoardGenerator
        var generator = new BoardGenerator(random);

        // CascadeProcessor
        var removeMatchedTilesProcessor = new RemoveMatchedTilesProcessor();
        var gravityProcessor = new GravityProcessor();
        var spawnProcessor = new SpawnProcessor(random);

        var cascadeProcessor = new CascadeProcessor(
            matchResolver,
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
