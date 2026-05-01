using UnityEngine;
public interface IBoardTransition
{
}
public readonly struct SwapTransition : IBoardTransition
{
    public readonly TilePosition From;

    public readonly TilePosition To;

    public SwapTransition(
        TilePosition from,
        TilePosition to)
    {
        From = from;
        To = to;
    }
}
public readonly struct RemoveTransition : IBoardTransition
{
    public readonly TilePosition Position;

    public RemoveTransition(TilePosition position)
    {
        Position = position;
    }
}
public readonly struct FallTransition : IBoardTransition
{
    public readonly TilePosition From;

    public readonly TilePosition To;

    public readonly ETileType TileType;

    public FallTransition(
        TilePosition from,
        TilePosition to,
        ETileType tileType)
    {
        From = from;
        To = to;
        TileType = tileType;
    }
}
public readonly struct SpawnTransition : IBoardTransition
{
    public readonly TilePosition Position;

    public readonly ETileType TileType;

    public SpawnTransition(
        TilePosition position,
        ETileType tileType)
    {
        Position = position;
        TileType = tileType;
    }
}