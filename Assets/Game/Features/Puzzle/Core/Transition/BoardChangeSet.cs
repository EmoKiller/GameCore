using System.Collections.Generic;
using UnityEngine;

public sealed class BoardChangeSet
{
    public IReadOnlyList<IBoardTransition> Transitions => _transitions;

    private readonly List<IBoardTransition> _transitions = new();
    private readonly HashSet<TilePosition> _protectedPositions = new();
    // private readonly HashSet<TilePosition> _movedPositions = new();

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
    // public void MarkMoved(TilePosition position)
    // {
    //     _movedPositions.Add(position);
    // }
    // public bool WasMoved(TilePosition position)
    // {
    //     return _movedPositions.Contains(position);
    // }
}
