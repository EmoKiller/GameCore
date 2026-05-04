using System.Collections.Generic;
using UnityEngine;

public sealed class SpecialTileProcessor
{
    private readonly IMatchPatternAnalyzer _analyzer;

    private readonly ISpecialTileResolver _resolver;

    public SpecialTileProcessor(
        IMatchPatternAnalyzer analyzer,
        ISpecialTileResolver resolver)
    {
        _analyzer = analyzer;

        _resolver = resolver;
    }

    public void Process(
        PuzzleBoard board,
        MatchResult matchResult,
        BoardChangeSet changeSet,
        SwapContext swapContext,
        HashSet<TilePosition> movedPositions)
    {
        foreach (MatchCluster cluster in matchResult.Clusters)
        {
            SpecialSpawnResult result =_analyzer.Analyze(
                    cluster,
                    swapContext,
                    movedPositions);

            if (result.HasSpecial == false)
            {
                continue;
            }

            TileData original = board.Get(result.Position);

            TileSpecialData special = _resolver.Resolve(
                    original.Type,
                    result.Pattern);
            Debug.Log( "special : "+ special);
            if (special == null)
            {
                continue;
            }

            TileData specialTile = new TileData(original.Type, special);

            board.Set(result.Position, specialTile);

            changeSet.Add(
                new CreateSpecialTransition(
                    result.Position,
                    specialTile.Special));
            changeSet.Protect(result.Position);
        }
    }
}
