using System.Collections.Generic;

public readonly struct Match
{
    public IReadOnlyList<(int x, int y)> Positions { get; }
    public ETileType Type { get; }

    public Match(ETileType type, IReadOnlyList<(int, int)> positions)
    {
        Type = type;
        Positions = positions;
    }
}