using System.Collections.Generic;
using UnityEngine;
public interface IMatchResolver
{
    MatchResult Resolve(PuzzleBoard board);
}
public sealed class MatchResolver : IMatchResolver
{
    private const int MinMatchCount = 3;

    public MatchResult Resolve(PuzzleBoard board)
    {
        var groups =
            new List<MatchGroup>();

        ScanHorizontal(
            board,
            groups);

        ScanVertical(
            board,
            groups);

        List<MatchCluster> clusters = BuildClusters(groups);

        return new MatchResult(
            clusters);
    }
    private List<MatchCluster> BuildClusters(List<MatchGroup> groups)
    {
        var clusters =
            new List<MatchCluster>();

        foreach (MatchGroup group in groups)
        {
            MatchCluster overlapCluster =
                null;

            foreach (MatchCluster cluster
                in clusters)
            {
                if (cluster.Overlaps(group))
                {
                    overlapCluster = cluster;

                    break;
                }
            }

            if (overlapCluster == null)
            {
                clusters.Add(
                    new MatchCluster(
                        group.TileType,
                        group.Positions));

                continue;
            }

            overlapCluster.Merge(group);
        }

        return clusters;
    }
    private void ScanHorizontal(
        PuzzleBoard board,
        List<MatchGroup> groups)
    {
        for (int y = 0; y < board.Height; y++)
        {
            int matchStartX = 0;
            int matchLength = 1;

            for (int x = 1; x < board.Width; x++)
            {
                var current = board.Get(x, y);
                var previous = board.Get(x - 1, y);

                if (current.Type == previous.Type &&
                    current.IsEmpty == false)
                {
                    matchLength++;
                }
                else
                {
                    TryCreateHorizontalGroup(
                        board,
                        y,
                        matchStartX,
                        matchLength,
                        groups);

                    matchStartX = x;
                    matchLength = 1;
                }
            }

            TryCreateHorizontalGroup(
                board,
                y,
                matchStartX,
                matchLength,
                groups);
        }
    }
 
    private void ScanVertical(
        PuzzleBoard board,
        List<MatchGroup> groups)
    {
        for (int x = 0; x < board.Width; x++)
        {
            int matchStartY = 0;
            int matchLength = 1;

            for (int y = 1; y < board.Height; y++)
            {
                var current = board.Get(x, y);
                var previous = board.Get(x, y - 1);

                if (current.Type == previous.Type &&
                    current.IsEmpty == false)
                {
                    matchLength++;
                }
                else
                {
                    TryCreateVerticalGroup(
                        board,
                        x,
                        matchStartY,
                        matchLength,
                        groups);

                    matchStartY = y;
                    matchLength = 1;
                }
            }

            TryCreateVerticalGroup(
                board,
                x,
                matchStartY,
                matchLength,
                groups);
        }
    }

    private void TryCreateHorizontalGroup(
        PuzzleBoard board,
        int y,
        int startX,
        int length,
        List<MatchGroup> groups)
    {
        if (length < MinMatchCount)
        {
            return;
        }

        var tileType = board.Get(startX, y).Type;

        var positions = new List<TilePosition>(length);

        for (int i = 0; i < length; i++)
        {
            positions.Add(
                new TilePosition(startX + i, y));
        }

        groups.Add(
            new MatchGroup(tileType, positions));
    }

    private void TryCreateVerticalGroup(
        PuzzleBoard board,
        int x,
        int startY,
        int length,
        List<MatchGroup> groups)
    {
        if (length < MinMatchCount)
        {
            return;
        }

        var tileType = board.Get(x, startY).Type;

        var positions = new List<TilePosition>(length);

        for (int i = 0; i < length; i++)
        {
            positions.Add(
                new TilePosition(x, startY + i));
        }

        groups.Add(
            new MatchGroup(tileType, positions));
    }
}
