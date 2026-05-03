using System.Collections.Generic;
using UnityEngine;

public sealed class MatchResult
{
    public IReadOnlyList<MatchCluster> Clusters => _clusters;

    private readonly List<MatchCluster> _clusters;

    public bool HasMatches => _clusters.Count > 0;

    public MatchResult( List<MatchCluster> clusters)
    {
        _clusters = clusters;
    }
}
