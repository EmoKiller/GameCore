

using System;

public readonly struct TilePosition : IEquatable<TilePosition>
{
    public readonly int X;
    public readonly int Y;

    public TilePosition(int x, int y)
    {
        X = x;
        Y = y;
    }

    public bool Equals(TilePosition other)
    {
        return X == other.X && Y == other.Y;
    }

    public override bool Equals(object obj)
    {
        return obj is TilePosition other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public override string ToString()
    {
        return $"({X},{Y})";
    }
    public static TilePosition Invalid => new TilePosition(-1, -1);

    public bool IsValid => X >= 0 && Y >= 0;
}
