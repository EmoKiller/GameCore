using System.Collections.Generic;
using UnityEngine;

public sealed class SwapResult
{
    public bool Success { get; }

    public CascadeResult CascadeResult { get; }

    public SwapResult(
        bool success,
        CascadeResult cascadeResult
    )
    {
        Success = success;
        CascadeResult = cascadeResult;
    }
}
