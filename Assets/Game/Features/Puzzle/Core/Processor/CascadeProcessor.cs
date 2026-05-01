using System.Collections.Generic;

public sealed class CascadeProcessor
{
    private readonly IMatchResolver _matchResolver;

    private readonly RemoveMatchedTilesProcessor _removeProcessor;

    private readonly GravityProcessor _gravityProcessor;

    private readonly SpawnProcessor _spawnProcessor;

    public CascadeProcessor(
        IMatchResolver matchResolver,
        RemoveMatchedTilesProcessor removeProcessor,
        GravityProcessor gravityProcessor,
        SpawnProcessor spawnProcessor)
    {
        _matchResolver = matchResolver;
        _removeProcessor = removeProcessor;
        _gravityProcessor = gravityProcessor;
        _spawnProcessor = spawnProcessor;
    }

    public CascadeResult Process(PuzzleBoard board)
    {
        var steps = new List<CascadeStepResult>();

        while (true)
        {
            var matchResult =
                _matchResolver.Resolve(board);

            if (matchResult.HasMatches == false)
            {
                break;
            }

            var changeSet = new BoardChangeSet();

            _removeProcessor.Remove(
                board,
                matchResult,
                changeSet);

            _gravityProcessor.Apply(
                board,
                changeSet);

            _spawnProcessor.FillEmpty(
                board,
                changeSet);

            steps.Add(
                new CascadeStepResult(
                    matchResult,
                    changeSet));
        }

        return new CascadeResult(steps);
    }
}