using System.Collections.Generic;

public sealed class CascadeProcessor
{
    private readonly IMatchResolver _matchResolver;

    private readonly SpecialTileProcessor _specialTileProcessor;

    private readonly RemoveMatchedTilesProcessor _removeProcessor;

    private readonly GravityProcessor _gravityProcessor;

    private readonly SpawnProcessor _spawnProcessor;

    private readonly HashSet<TilePosition> _movedPositions = new();
    public HashSet<TilePosition> MovedPositions => _movedPositions;

    public CascadeProcessor(
        IMatchResolver matchResolver,
        SpecialTileProcessor specialTileProcessor,
        RemoveMatchedTilesProcessor removeProcessor,
        GravityProcessor gravityProcessor,
        SpawnProcessor spawnProcessor)
    {
        _matchResolver = matchResolver;
        _specialTileProcessor = specialTileProcessor;
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
        SwapContext swapContext
    )
    {
        _specialTileProcessor.Process(
            board,
            matchResult,
            changeSet,
            swapContext,
            _movedPositions);

        _removeProcessor.Remove(
            board,
            matchResult,
            changeSet);

        _gravityProcessor.Apply(
            board,
            changeSet,
            _movedPositions);

        _spawnProcessor.FillEmpty(
            board,
            changeSet,
            _movedPositions);
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