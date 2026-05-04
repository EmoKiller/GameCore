using System.Collections.Generic;
using UnityEngine;

public sealed class BoardChangeSet
{
    public IReadOnlyList<IBoardTransition> Transitions => _transitions;

    private readonly List<IBoardTransition> _transitions = new();
    private readonly HashSet<TilePosition> _protectedPositions = new();
    private readonly HashSet<TilePosition> _removedPositions = new();

    public void Add(IBoardTransition transition)
    {
        _transitions.Add(transition);
    }
    public bool IsProtected(TilePosition position)
    {
        return _protectedPositions.Contains(position);
    }
    public void Protect(TilePosition position)
    {
        _protectedPositions.Add(position);
    }
    public bool IsRemoved(TilePosition position)
    {
        return _removedPositions.Contains(
            position);
    }

    public void MarkRemoved(
        TilePosition position)
    {
        _removedPositions.Add(position);
    }
}
