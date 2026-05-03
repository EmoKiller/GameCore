using System.Collections.Generic;
using System.Linq;

public interface IMatchPatternAnalyzer
{
    SpecialSpawnResult Analyze(MatchCluster cluster,SwapContext swapContext, HashSet<TilePosition> movedPositions);
}
public sealed class MatchPatternAnalyzer : IMatchPatternAnalyzer
{
    public SpecialSpawnResult Analyze(
        MatchCluster cluster,
        SwapContext swapContext,
        HashSet<TilePosition> movedPositions)
    {
        int horizontalLength =
            GetMaxHorizontal(cluster);

        int verticalLength =
            GetMaxVertical(cluster);

        TilePosition spawnPosition =
            ResolveSpawnPosition(
                cluster,
                swapContext,
                movedPositions);

        if (horizontalLength >= 5 ||
            verticalLength >= 5)
        {
            return new SpecialSpawnResult(
                true,
                spawnPosition,
                ETileSpecialType.ColorBomb);
        }

        bool hasHorizontal =
            horizontalLength >= 3;

        bool hasVertical =
            verticalLength >= 3;

        if (hasHorizontal &&
            hasVertical)
        {
            return new SpecialSpawnResult(
                true,
                spawnPosition,
                ETileSpecialType.Bomb);
        }

        if (horizontalLength == 4)
        {
            return new SpecialSpawnResult(
                true,
                spawnPosition,
                ETileSpecialType.HorizontalRocket);
        }

        if (verticalLength == 4)
        {
            return new SpecialSpawnResult(
                true,
                spawnPosition,
                ETileSpecialType.VerticalRocket);
        }

        return SpecialSpawnResult.None();
    }
    private TilePosition ResolveSpawnPosition(
        MatchCluster cluster,
        SwapContext swapContext,
        HashSet<TilePosition> movedPositions)
    {
        if (cluster.Positions.Contains(
            swapContext.To))
        {
            return swapContext.To;
        }

        if (cluster.Positions.Contains(
            swapContext.From))
        {
            return swapContext.From;
        }

        foreach (TilePosition pos in cluster.Positions)
        {
            if (movedPositions.Contains(pos))
            {
                return pos;
            }
        }

        return cluster.AnyPosition;
    }
    private int GetMaxHorizontal(MatchCluster cluster)
    {
        int max = 0;

        foreach (TilePosition pivot in cluster.Positions)
        {
            int count = 0;

            foreach (TilePosition other
                in cluster.Positions)
            {
                if (other.Y == pivot.Y)
                {
                    count++;
                }
            }

            if (count > max)
            {
                max = count;
            }
        }

        return max;
    }
    private int GetMaxVertical(MatchCluster cluster)
    {
        int max = 0;

        foreach (TilePosition pivot in cluster.Positions)
        {
            int count = 0;

            foreach (TilePosition other in cluster.Positions)
            {
                if (other.X == pivot.X)
                {
                    count++;
                }
            }

            if (count > max)
            {
                max = count;
            }
        }

        return max;
    }
}