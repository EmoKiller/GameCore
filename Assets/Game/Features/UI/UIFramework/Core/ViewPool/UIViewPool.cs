using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Application.UI.Core.Abstractions;
using UI.Core.Factory;
using UnityEngine;
public interface IUIViewPool
{
    UniTask<IUIView> GetAsync(Type viewType, CancellationToken ct);
    void Release(Type viewType, IUIView view);
    UniTask WarmupAsync(Type viewType, int count, CancellationToken ct);
}
public sealed class UIViewPool : IUIViewPool
{
    private readonly UIViewFactory _factory;

    private readonly Dictionary<Type, Stack<IUIView>> _pool = new();
    private readonly Dictionary<Type, int> _capacity = new();
    private readonly IUIProfilerService _profiler;
    public UIViewPool(UIViewFactory factory, IUIProfilerService profiler)
    {
        _factory = factory;
        _profiler = profiler;
    }

    public void SetCapacity(Type viewType, int capacity)
    {
        _capacity[viewType] = capacity;
    }

    public async UniTask<IUIView> GetAsync(Type viewType, CancellationToken ct)
    {
        if (_pool.TryGetValue(viewType, out var stack) && stack.Count > 0)
        {
            var view = stack.Pop();

            if (view is IReusableView reusable)
                reusable.OnBeforeReuse();

            ((Component)view).gameObject.SetActive(true);

            _profiler.Record(new UIProfilerEvent(
                UIProfilerEventType.ReuseFromPool,
                viewType,
                Time.time)); 

            return view;
        }
        return await _factory.CreateAsync(viewType, ct);
    }

    public void Release(Type viewType, IUIView view)
    {
        if (!_pool.TryGetValue(viewType, out var stack))
        {
            stack = new Stack<IUIView>();
            _pool[viewType] = stack;
        }
        
        var cap = _capacity.TryGetValue(viewType, out var c) ? c : 2; 
        if (stack.Count >= cap) 
        { 
            UnityEngine.Object.Destroy(((Component)view).gameObject);
            _profiler.Record(new UIProfilerEvent(
                UIProfilerEventType.Destroy,
                viewType,
                Time.time
                ));
            return;
        }

        ((Component)view).gameObject.SetActive(false);
        
        stack.Push(view);

        _profiler.Record(new UIProfilerEvent( 
            UIProfilerEventType.ReleaseToPool,
            viewType,
            Time.time
            ));
    }

    public async UniTask WarmupAsync(Type viewType, int count, CancellationToken ct)
    {
        if (!_pool.TryGetValue(viewType, out var stack))
        {
            stack = new Stack<IUIView>();
            _pool[viewType] = stack;
        }

        var cap = _capacity.TryGetValue(viewType, out var c) ? c : count;

        while (stack.Count < Math.Min(count, cap))
        {
            var view = await _factory.CreateAsync(viewType, ct);

            ((Component)view).gameObject.SetActive(false);

            stack.Push(view);
        }
    }
}
