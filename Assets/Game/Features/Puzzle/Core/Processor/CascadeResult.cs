using System.Collections.Generic;
using UnityEngine;

public sealed class CascadeResult
{
    public IReadOnlyList<CascadeStepResult> Steps => _steps;

    private readonly List<CascadeStepResult> _steps;

    public int ChainCount => _steps.Count;

    public CascadeResult(List<CascadeStepResult> steps)
    {
        _steps = steps;
    }
}
