using System.Collections.Generic;

public sealed class CascadeProcessor
{
    private readonly IMatchResolver _matchResolver;

    private readonly SpecialTileProcessor _specialTileProcessor;

    private readonly ISpecialActivationChainProcessor _specialActivationChainProcessor;

    private readonly RemoveMatchedTilesProcessor _removeProcessor;

    private readonly GravityProcessor _gravityProcessor;

    private readonly SpawnProcessor _spawnProcessor;
    
    private List<SpecialActivationRequest> _persistentActivations = new();
    
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
        SwapContext swapContext)
    {

        var steps = new List<CascadeStepResult>();

        MatchResult match = _matchResolver.Resolve(board);

        if (!match.HasMatches)
            return new CascadeResult(steps);

        var changeSet = initialChangeSet;

        ProcessStep(board, match, changeSet, swapContext);

        steps.Add(new CascadeStepResult(match, changeSet));

        while (true)
        {
            MatchResult nextMatch = _matchResolver.Resolve(board);

            if (!nextMatch.HasMatches && _persistentActivations.Count == 0)
                break;

            var nextChangeSet = new BoardChangeSet();

            ProcessStep(
                board,
                nextMatch,
                nextChangeSet,
                default);

            steps.Add(new CascadeStepResult(nextMatch, nextChangeSet));
        }

        return new CascadeResult(steps);
    }
    
    private void ProcessStep(
        PuzzleBoard board,
        MatchResult matchResult,
        BoardChangeSet changeSet,
        SwapContext swapContext)
    {
        List<SpecialActivationRequest> activations = matchResult.GetSpecialActivations(board);
        if (_persistentActivations.Count > 0)
        {
            activations.AddRange(_persistentActivations);
        }
        // if (_persistentActivations.Count > 0)
        // {
        //     activations.AddRange(_persistentActivations);
        // }

        // 3. Spawn new specials
        _specialTileProcessor.Process(
            board,
            matchResult,
            changeSet,
            swapContext);

        // 4. Activate chain
        
        SpecialChainProcessResult chainResult =_specialActivationChainProcessor.Process(
            board,
            activations,
            changeSet);

        

        // 6. REMOVE matched tiles
        _removeProcessor.Remove(
            board,
            matchResult,
            changeSet);

        // 7. GRAVITY
        _gravityProcessor.Apply(
            board,
            changeSet);

        // 8. SPAWN
        _spawnProcessor.FillEmpty(
            board,
            changeSet);

        _persistentActivations.Clear();
        foreach (TileData tile in chainResult.PersistentTiles)
        {
            if (tile.Position.IsValid == false)
            {
                continue;
            }
            tile.RuntimeSpecialState.LifecycleState = ESpecialLifecycleState.None;
            _persistentActivations.Add(
                new SpecialActivationRequest(
                    tile.Position,
                    tile));
        }
    }
    
}