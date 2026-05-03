using UnityEngine;

public sealed class SpecialTileProcessor
{
    private readonly IMatchPatternAnalyzer _analyzer;

    private readonly SpecialTileFactory _factory;

    public SpecialTileProcessor(
        IMatchPatternAnalyzer analyzer,
        SpecialTileFactory factory)
    {
        _analyzer = analyzer;

        _factory = factory;
    }

    public void Process(
        PuzzleBoard board,
        MatchResult result,
        BoardChangeSet changeSet,
        SwapContext swapContext)
    {
        foreach (MatchGroup group in result.Groups)
        {
            SpecialSpawnResult spawn = _analyzer.Analyze(group, swapContext);

            if (spawn.HasSpecial == false)
            {
                continue;
            }

            ETileType baseType = group.TileType;

            _factory.CreateSpecial(
                board,
                spawn,
                baseType
            );
            Debug.Log($"Spawn Rocket at {spawn.SpawnPosition}");
            changeSet.Protect(spawn.SpawnPosition);

            changeSet.Add(new CreateSpecialTransition(
                spawn.SpawnPosition,
                spawn.SpecialType
            ));
        }
    }
}
