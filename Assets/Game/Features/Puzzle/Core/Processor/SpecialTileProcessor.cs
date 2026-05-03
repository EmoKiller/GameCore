using System.Collections.Generic;
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
        SwapContext swapContext,
        HashSet<TilePosition> movedPositions)
    {
        foreach (MatchCluster group in result.Clusters)
        {
            SpecialSpawnResult spawn = _analyzer.Analyze(group, swapContext, movedPositions);

            if (spawn.HasSpecial == false)
            {
                continue;
            }

            TileData specialTile = _factory.CreateSpecial(
                    board,
                    spawn.SpawnPosition,
                    spawn.SpecialType);
            changeSet.Protect(spawn.SpawnPosition);
            changeSet.Add(
                new CreateSpecialTransition(
                    spawn.SpawnPosition,
                    specialTile));
        }
    }
}
