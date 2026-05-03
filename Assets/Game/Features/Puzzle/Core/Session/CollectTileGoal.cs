using UnityEngine;

public sealed class CollectTileGoal :IPuzzleGoal
{
    private readonly ETileType _targetType;

    private int _remaining;

    public bool IsCompleted =>
        _remaining <= 0;

    public CollectTileGoal(
        ETileType targetType,
        int amount)
    {
        _targetType = targetType;
        _remaining = amount;
    }

    public void Process(MatchResult matchResult)
    {
        // foreach (TileMatch match in matchResult.Matches)
        // {
        //     foreach (TilePosition position in match.Positions)
        //     {
        //         if (match.TileType != _targetType)
        //         {
        //             continue;
        //         }

        //         _remaining--;
        //     }
        // }
    }
}