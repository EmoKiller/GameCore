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

        if (horizontalLength >= 5 || verticalLength >= 5)
        {
            return new SpecialSpawnResult(
                true,
                spawnPosition,
                ETileSpecialType.ColorBomb);
        }

        bool hasHorizontal = horizontalLength >= 3;

        bool hasVertical = verticalLength >= 3;

        if (hasHorizontal && hasVertical)
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
        if (TryGetIntersection(cluster, out TilePosition intersection))
        {
            return intersection;
        }
        if (cluster.Positions.Contains(swapContext.To))
        {
            return swapContext.To;
        }

        if (cluster.Positions.Contains(swapContext.From))
        {
            return swapContext.From;
        }

        TilePosition center = GetClusterCenter(cluster);

        TilePosition? bestMoved =
            null;

        float bestDistance =
            float.MaxValue;

        foreach (TilePosition pos
            in movedPositions)
        {
            if (cluster.Positions.Contains(pos)
                == false)
            {
                continue;
            }

            float dx =
                pos.X - center.X;

            float dy =
                pos.Y - center.Y;

            float sqr =
                dx * dx +
                dy * dy;

            if (sqr < bestDistance)
            {
                bestDistance = sqr;
                bestMoved = pos;
            }
        }

        if (bestMoved.HasValue)
        {
            return bestMoved.Value;
        }

        return center;
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
    private bool TryGetIntersection(
        MatchCluster cluster,
        out TilePosition intersection)
    {
        foreach (TilePosition pivot
            in cluster.Positions)
        {
            bool hasHorizontal = false;
            bool hasVertical = false;

            foreach (TilePosition other
                in cluster.Positions)
            {
                if (other.Y == pivot.Y &&
                    other.X != pivot.X)
                {
                    hasHorizontal = true;
                }

                if (other.X == pivot.X &&
                    other.Y != pivot.Y)
                {
                    hasVertical = true;
                }
            }

            if (hasHorizontal &&
                hasVertical)
            {
                intersection = pivot;
                return true;
            }
        }

        intersection = default;
        return false;
    }
    private TilePosition GetClusterCenter(MatchCluster cluster)
    {
        float avgX = 0f;
        float avgY = 0f;

        int count = 0;

        foreach (TilePosition pos in cluster.Positions)
        {
            avgX += pos.X;
            avgY += pos.Y;

            count++;
        }

        avgX /= count;
        avgY /= count;

        TilePosition best = default;

        float bestDistance = float.MaxValue;

        foreach (TilePosition pos
            in cluster.Positions)
        {
            float dx = pos.X - avgX;

            float dy = pos.Y - avgY;

            float sqr =
                dx * dx +
                dy * dy;

            if (sqr < bestDistance)
            {
                bestDistance = sqr;
                best = pos;
            }
        }

        return best;
    }
}