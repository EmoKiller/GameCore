using System.Collections.Generic;
using UnityEngine;

public sealed class MatchResult
{
    public IReadOnlyList<MatchGroup> Groups => _groups;

    private readonly List<MatchGroup> _groups;

    public bool HasMatches => _groups.Count > 0;

    public MatchResult(List<MatchGroup> groups)
    {
        _groups = groups;
    }
}
