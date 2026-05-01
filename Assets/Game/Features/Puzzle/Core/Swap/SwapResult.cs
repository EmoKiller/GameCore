using System.Collections.Generic;
using UnityEngine;

public sealed class SwapResult
{
    public bool Success { get; }

    public IReadOnlyList<BoardChangeSet> ChangeSets => _changeSets;

    private readonly List<BoardChangeSet>
        _changeSets;

    public SwapResult(
        bool success,
        List<BoardChangeSet> changeSets)
    {
        Success = success;
        _changeSets = changeSets;
    }
}
