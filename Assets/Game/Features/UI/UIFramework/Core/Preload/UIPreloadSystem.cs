using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

public interface IUIPreloadSystem
{
    UniTask PreloadAsync(Type viewType,int poolWarmupSize, CancellationToken ct);
}

public sealed class UIPreloadSystem : IUIPreloadSystem
{
    private readonly UIPreloadGraph _graph;
    private readonly UIViewPool _pool;

    private readonly HashSet<Type> _loaded = new();
    private readonly HashSet<Type> _visiting = new(); // detect cycle

    public UIPreloadSystem(
        UIPreloadGraph graph,
        UIViewPool pool)
    {
        _graph = graph;
        _pool = pool;
    }

    public async UniTask PreloadAsync(Type viewType,int poolWarmupSize, CancellationToken ct)
    {
        if (_loaded.Contains(viewType))
            return;

        if (_visiting.Contains(viewType))
            throw new InvalidOperationException(
                $"Circular dependency detected: {viewType}");

        _visiting.Add(viewType);

        var node = _graph.Get(viewType);

        if (node != null)
        {
            foreach (var dep in node.Dependencies)
            {
                await PreloadAsync(dep,poolWarmupSize, ct);
            }
        }

        await _pool.WarmupAsync(viewType, poolWarmupSize, ct);

        _visiting.Remove(viewType);
        _loaded.Add(viewType);
    }
}