using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

public sealed class LoadingPipeline
{
    private readonly List<ILoadingMiddleware> _middlewares = new();

    public LoadingPipeline Use(ILoadingMiddleware middleware)
    {
        _middlewares.Add(middleware);
        return this;
    }

    public int Count => _middlewares.Count;

    public async UniTask ExecuteAsync(LoadingContext ctx, CancellationToken ct)
    {
        // 1. Build weights (NO LINQ, Unity-safe)
        var weights = new float[_middlewares.Count];

        for (int i = 0; i < _middlewares.Count; i++)
        {
            var m = _middlewares[i];

            weights[i] = m is IWeightedLoadingMiddleware w
                ? w.Weight
                : 1f;
        }

        // 2. Normalize weights
        float total = 0f;
        for (int i = 0; i < weights.Length; i++)
            total += weights[i];

        if (total <= 0f)
            total = 1f;

        for (int i = 0; i < weights.Length; i++)
            weights[i] /= total;

        // 3. Initialize progress aggregator
        ctx.Progress.Initialize(weights);

        // 4. Execute middleware chain
        int index = -1;

        UniTask Next()
        {
            index++;

            if (index < _middlewares.Count)
            {
                return _middlewares[index].InvokeAsync(ctx, Next, ct);
            }

            return UniTask.CompletedTask;
        }

        await Next();
    }
}