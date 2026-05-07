using System.Collections.Generic;
using System.Linq;

public interface IMatchPatternAnalyzer
{
    SpecialSpawnResult Analyze(PuzzleBoard board, MatchCluster cluster,SwapContext swapContext);
}
public sealed class MatchPatternAnalyzer : IMatchPatternAnalyzer
{
    public SpecialSpawnResult Analyze(
        PuzzleBoard board,
        MatchCluster cluster,
        SwapContext swapContext)
    {
        int horizontalLength =
            GetMaxHorizontal(cluster);

        int verticalLength =
            GetMaxVertical(cluster);

        TilePosition spawnPosition =
            ResolveSpawnPosition(
                board,
                cluster,
                swapContext);

        if (horizontalLength >= 5 || verticalLength >= 5)
        {
            return new SpecialSpawnResult(
                true,
                spawnPosition,
                EMatchPatternType.Line5);
        }

        bool hasHorizontal = horizontalLength >= 3;

        bool hasVertical = verticalLength >= 3;

        if (hasHorizontal && hasVertical)
        {
            return new SpecialSpawnResult(
                true,
                spawnPosition,
                EMatchPatternType.TShape);
        }

        if (horizontalLength == 4)
        {
            return new SpecialSpawnResult(
                true,
                spawnPosition,
                EMatchPatternType.Line4Horizontal);
        }

        if (verticalLength == 4)
        {
            return new SpecialSpawnResult(
                true,
                spawnPosition,
                EMatchPatternType.Line4Vertical);
        }

        return SpecialSpawnResult.None();
    }
    private TilePosition ResolveSpawnPosition(
        PuzzleBoard board,
        MatchCluster cluster,
        SwapContext swapContext)
    {
        if (TryGetIntersection(cluster, out TilePosition intersection))
        {
            return intersection;
        }

        if (cluster.Positions.Contains(swapContext.To))
        {
            TileData toTile = board.Get(swapContext.To);

            if (toTile.HasSpecial == false)
            {
                return swapContext.To;
            }

            return ResolveFallbackPosition(
                board,
                cluster,
                swapContext.To);
        }

        if (cluster.Positions.Contains(swapContext.From))
        {
            TileData fromTile = board.Get(swapContext.From);

            if (fromTile.HasSpecial == false)
            {
                return swapContext.From;
            }

            return ResolveFallbackPosition(
                board,
                cluster,
                swapContext.From);
        }

        return GetClusterCenter(cluster);
    }
    private TilePosition ResolveFallbackPosition(
        PuzzleBoard board,
        MatchCluster cluster,
        TilePosition blocked)
    {
        foreach (TilePosition pos in cluster.Positions)
        {
            if (pos.Equals(blocked))
            {
                continue;
            }

            TileData tile = board.Get(pos);

            if (tile.HasSpecial)
            {
                continue;
            }

            return pos;
        }

        return blocked;
    }
    private int GetMaxHorizontal(MatchCluster cluster)
    {
        int max = 0;

        foreach (TilePosition pivot in cluster.Positions)
        {
            int count = 0;

            foreach (TilePosition other in cluster.Positions)
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