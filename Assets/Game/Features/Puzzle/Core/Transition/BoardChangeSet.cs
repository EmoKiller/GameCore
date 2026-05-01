using System.Collections.Generic;
using UnityEngine;

public sealed class BoardChangeSet
{
    public IReadOnlyList<IBoardTransition> Transitions => _transitions;

    private readonly List<IBoardTransition> _transitions = new();

    public void Add(IBoardTransition transition)
    {
        _transitions.Add(transition);
    }
}
