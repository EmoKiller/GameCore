using System.Collections.Generic;
using System.Linq;

public sealed class CascadeProcessor
{
    private readonly IMatchResolver _matchResolver;

    private readonly SpecialTileProcessor _specialTileProcessor;
    private readonly ISpecialActivationChainProcessor _specialActivationProcessor;

    private readonly RemoveMatchedTilesProcessor _removeProcessor;

    private readonly GravityProcessor _gravityProcessor;

    private readonly SpawnProcessor _spawnProcessor;

    private readonly HashSet<TilePosition> _movedPositions = new();
    public HashSet<TilePosition> MovedPositions => _movedPositions;

    public CascadeProcessor(
        IMatchResolver matchResolver,
        SpecialTileProcessor specialTileProcessor,
        ISpecialActivationChainProcessor specialActivationProcessor,
        RemoveMatchedTilesProcessor removeProcessor,
        GravityProcessor gravityProcessor,
        SpawnProcessor spawnProcessor)
    {
        _matchResolver = matchResolver;
        _specialTileProcessor = specialTileProcessor;
        _specialActivationProcessor = specialActivationProcessor;
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
        IEnumerable<TilePosition> activatedSpecials = matchResult.GetSpecialPositions(board).ToList();

        _specialTileProcessor.Process(
            board,
            matchResult,
            changeSet,
            swapContext,
            _movedPositions);

        _specialActivationProcessor.Process(
            board,
            activatedSpecials,
            changeSet);

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