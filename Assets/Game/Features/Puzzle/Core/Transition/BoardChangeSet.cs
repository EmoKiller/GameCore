using System.Collections.Generic;
using UnityEngine;

public sealed class BoardChangeSet
{
    public IReadOnlyList<IBoardTransition> Transitions => _transitions;

    private readonly List<IBoardTransition> _transitions = new();
    private readonly HashSet<TilePosition> _protectedPositions = new();


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
}
