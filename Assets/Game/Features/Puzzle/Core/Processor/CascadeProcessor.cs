using System.Collections.Generic;
using System.Linq;

public sealed class CascadeProcessor
{
    private readonly IMatchResolver _matchResolver;

    private readonly SpecialTileProcessor _specialTileProcessor;

    private readonly ISpecialActivationChainProcessor _specialActivationChainProcessor;

    private readonly RemoveMatchedTilesProcessor _removeProcessor;

    private readonly GravityProcessor _gravityProcessor;

    private readonly SpawnProcessor _spawnProcessor;

    private readonly HashSet<TilePosition> _movedPositions = new();
    public HashSet<TilePosition> MovedPositions => _movedPositions;


    public CascadeProcessor(
        IMatchResolver matchResolver,
        SpecialTileProcessor specialTileProcessor,
        ISpecialActivationChainProcessor specialActivationChainProcessor,
        RemoveMatchedTilesProcessor removeProcessor,
        GravityProcessor gravityProcessor,
        SpawnProcessor spawnProcessor)
    {
        _matchResolver = matchResolver;
        _specialTileProcessor = specialTileProcessor;
        _specialActivationChainProcessor = specialActivationChainProcessor;
        _removeProcessor = removeProcessor;
        _gravityProcessor = gravityProcessor;
        _spawnProcessor = spawnProcessor;
    }

    public CascadeResult Process(
        PuzzleBoard board,
        BoardChangeSet initialChangeSet,
        SwapContext swapContext
    )
    {
        _movedPositions.Clear();
        var steps = new List<CascadeStepResult>();

        MatchResult initialMatch = _matchResolver.Resolve(board);

        if (initialMatch.HasMatches)
        {
            ProcessStep(
                board,
                initialMatch,
                initialChangeSet,
                swapContext);

            steps.Add( new CascadeStepResult(initialMatch, initialChangeSet));
        }

        while (true)
        {
            MatchResult matchResult = _matchResolver.Resolve(board);

            if (matchResult.HasMatches == false)
            {
                break;
            }

            var changeSet = new BoardChangeSet();
            ProcessStep(
                board,
                matchResult,
                changeSet,
                default
            );

            steps.Add( new CascadeStepResult(matchResult, changeSet));
        }
        _movedPositions.Clear();
        return new CascadeResult(steps);
    }

    private void ProcessStep(
        PuzzleBoard board,
        MatchResult matchResult,
        BoardChangeSet changeSet,
        SwapContext swapContext)
    {
        // 1. Collect match-triggered specials
        List<SpecialActivationRequest> activations = matchResult.GetSpecialActivations(board);

        // 2. Inject persistent specials (auto retrigger)
        // if (_persistentActivations.Count > 0)
        // {
        //     activations.AddRange(_persistentActivations);
        // }

        // 3. Spawn new specials
        _specialTileProcessor.Process(
            board,
            matchResult,
            changeSet,
            swapContext,
            _movedPositions);

        // 4. Activate chain
        
        _specialActivationChainProcessor.Process(
            board,
            activations,
            changeSet);

        // 5. CLEAR persistent list
        // _persistentActivations.Clear();

        // 6. REMOVE matched tiles
        _removeProcessor.Remove(
            board,
            matchResult,
            changeSet);

        // 7. GRAVITY
        _gravityProcessor.Apply(
            board,
            changeSet,
            _movedPositions);

        // 8. SPAWN
        _spawnProcessor.FillEmpty(
            board,
            changeSet,
            _movedPositions);

        // 9. REBUILD persistent activations 
        // foreach (TileData tile in chainResult.PersistentTiles)
        // {
        //     TilePosition pos = board.FindPosition(tile);

        //     if (pos.IsValid == false)
        //     {
        //         continue;
        //     }

        //     _persistentActivations.Add(
        //         new SpecialActivationRequest(pos, tile));
        // }
    }
    
    public void Add(TilePosition position)
    {
        _movedPositions.Add(position);
    }
    public bool WasMoved(TilePosition position)
    {
        return _movedPositions.Contains(position);
    }
    
}