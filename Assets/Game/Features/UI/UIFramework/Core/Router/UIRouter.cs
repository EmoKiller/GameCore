using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Application.UI;
using Game.Application.UI.Core.Abstractions;
using Game.Presentation.UI.Data;
using UnityEngine;

public interface IUIRouter
{
    // =====================================================
    // PUSH
    // =====================================================

    UniTask<UIHandle> PushAsync( Type viewType, CancellationToken ct);

    UniTask<UIHandle> PushAsync<TView>( CancellationToken ct) where TView : class, IUIView;

    // =====================================================
    // POP
    // =====================================================

    UniTask PopAsync( Type viewType, CancellationToken ct);

    UniTask PopTopAsync( CancellationToken ct);
    // =====================================================
    // OVERLAY
    // =====================================================
    UniTask RemoveOverlayAsync(Type viewType, CancellationToken ct);

    // =====================================================
    // REPLACE (Navigator dùng)
    // =====================================================

    UniTask<UIHandle> ReplaceAsync( Type viewType, CancellationToken ct);

    // =====================================================
    // QUERY
    // =====================================================

    UIHandle Peek();

    bool HasAny();
}

public sealed class UIRouter : IUIRouter
{
    private readonly IUIPresentationService _presentation;
    private readonly UIManifest _manifest;
    private readonly IUIRetentionCache _cache;
    private readonly IUIProfilerService _profiler;
    private readonly IUIRuntimeValidator _validator;

    private readonly LinkedList<UIHandle> _screenList = new();
    private readonly LinkedList<UIHandle> _modalList = new();
    private readonly Dictionary<Type, UIHandle> _overlays = new();

    private readonly SemaphoreSlim _lock = new(1, 1);

    public UIRouter(
        IUIPresentationService presentation,
        UIManifest manifest,
        IUIRetentionCache cache,
        IUIProfilerService profiler,
        IUIRuntimeValidator validator)
    {
        _presentation = presentation;
        _manifest = manifest;
        _cache = cache;
        _profiler = profiler;
        _validator = validator;
    }

    // =====================================================
    // PUSH
    // =====================================================

    public async UniTask<UIHandle> PushAsync(Type viewType, CancellationToken ct)
    {
        await _lock.WaitAsync(ct);

        try
        {
            var entry = _manifest.Get(viewType);

            // ===== DUPLICATE GUARD =====
            if (IsTopSame(viewType, entry.Layer))
                return Peek();

            UIHandle handle;

            // ===== CACHE HIT =====
            if (_cache.TryGet(viewType, out var cached))
            {
                _validator?.ValidateCacheRetrieve(cached);

                _cache.Remove(viewType);

                var view = cached.Instance.View;

                // =========================
                // RESET TRƯỚC KHI DÙNG LẠI
                // =========================
                if (view is IReusableView reusable)
                {
                    reusable.OnBeforeReuse(); // ✔ ĐÚNG
                }

                await view.ShowAsync(ct);

                cached.State = UIHandleState.Active;

                handle = cached;
            }
            else
            {
                handle = await _presentation.PresentAsync(viewType, ct);

                RemoveFromAllContainers(handle);

                handle.State = UIHandleState.Active;

                _validator?.ValidatePush(handle, viewType);
            }

            // ===== ATTACH (CHUNG) =====
            await AttachToLayerAsync(handle, entry, ct);

            return handle;
        }
        finally
        {
            _lock.Release();
        }
    }
    private async UniTask AttachToLayerAsync(
        UIHandle handle,
        UIManifestEntry entry,
        CancellationToken ct)
    {
        var viewType = handle.Instance.View.GetType();

        switch (entry.Layer)
        {
            case EUILayer.Screen:
                await HandleScreenAsync(handle, ct);
                break;

            case EUILayer.Popup:
                _modalList.AddLast(handle);
                break;

            case EUILayer.Overlay:
                await HandleOverlayAsync(viewType, handle, ct);
                break;
        }
    }

    public UniTask<UIHandle> PushAsync<TView>(CancellationToken ct)
        where TView : class, IUIView
        => PushAsync(typeof(TView), ct);

    // =====================================================
    // POP TOP
    // =====================================================
    public async UniTask PopAsync(
        Type viewType,
        CancellationToken ct)
    {
        await _lock.WaitAsync(ct);

        try
        {
            var entry = _manifest.Get(viewType);

            switch (entry.Layer)
            {
                case EUILayer.Screen:
                    await PopScreenAsync(ct);
                    break;

                case EUILayer.Popup:
                    await PopModalByTypeAsync(viewType, ct);
                    break;

                case EUILayer.Overlay:
                    await RemoveOverlayAsync(viewType, ct);
                    break;
            }
        }
        finally
        {
            _lock.Release();
    }
}
    private async UniTask PopModalByTypeAsync(
        Type viewType,
        CancellationToken ct)
    {
        var node = _modalList.Last;

        while (node != null)
        {
            if (node.Value.Instance.View.GetType() == viewType)
            {
                var handle = node.Value;
                _modalList.Remove(node);

                _validator?.ValidatePop(handle);

                await _presentation.DismissAsync(handle, ct);
                return;
            }

            node = node.Previous;
        }
    }
    public async UniTask PopTopAsync(CancellationToken ct)
    {
        await _lock.WaitAsync(ct);

        try
        {
            if (_modalList.Count > 0)
            {
                await PopModalAsync(ct);
                return;
            }

            if (_screenList.Count > 0)
            {
                await PopScreenAsync(ct);
            }
        }
        finally
        {
            _lock.Release();
        }
    }

    // =====================================================
    // SCREEN
    // =====================================================

    private async UniTask HandleScreenAsync(UIHandle handle, CancellationToken ct)
    {
        var current = _screenList.Last?.Value;

        if (current != null)
        {
            await current.Instance.View.HideAsync(ct);
            current.State = UIHandleState.Hidden;
        }

        _screenList.AddLast(handle);

        EnforceRetentionLimit(handle.Instance.View.GetType());
    }

    private async UniTask PopScreenAsync(CancellationToken ct)
    {
        if (_screenList.Count == 0)
            return;

        var node = _screenList.Last;
        var handle = node.Value;

        _validator?.ValidatePop(handle);

        var type = handle.Instance.View.GetType();
        var entry = _manifest.Get(type);

        _screenList.RemoveLast();

        await ProcessReusePolicy(handle, entry, ct);

        // restore previous
        var prev = _screenList.Last?.Value;

        if (prev != null)
        {
            prev.State = UIHandleState.Active;
            await prev.Instance.View.ShowAsync(ct);
        }
    }

    // =====================================================
    // MODAL
    // =====================================================

    private async UniTask PopModalAsync(CancellationToken ct)
    {
        if (_modalList.Count == 0)
            return;

        var handle = _modalList.Last.Value;

        _modalList.RemoveLast();

        _validator?.ValidatePop(handle);

        await _presentation.DismissAsync(handle, ct);
    }

    // =====================================================
    // OVERLAY
    // =====================================================

    private async UniTask HandleOverlayAsync(
        Type viewType,
        UIHandle handle,
        CancellationToken ct)
    {
        if (_overlays.TryGetValue(viewType, out var existing))
        {
            await RemoveOverlayInternalAsync(viewType, existing, ct);
        }

        _overlays[viewType] = handle;
        handle.State = UIHandleState.Active;
    }

    
    public async UniTask RemoveOverlayAsync(
        Type viewType,
        CancellationToken ct)
    {
        if (!_overlays.TryGetValue(viewType, out var handle))
                return;

            await RemoveOverlayInternalAsync(viewType, handle, ct);
            _overlays.Remove(viewType);
    }
    private async UniTask RemoveOverlayInternalAsync(
        Type type,
        UIHandle handle,
        CancellationToken ct)
    {
        var entry = _manifest.Get(type);

        await ProcessReusePolicy(handle, entry, ct);
    }
    // =====================================================
    // REUSE POLICY CORE
    // =====================================================

    private async UniTask ProcessReusePolicy(
        UIHandle handle,
        UIManifestEntry entry,
        CancellationToken ct)
    {
        var view = handle.Instance.View;
        var type = view.GetType();

        switch (entry.ReusePolicy)
        {
            case UIReusePolicy.Retain:
                await view.HideAsync(ct);
                handle.State = UIHandleState.Hidden;
                break;

            case UIReusePolicy.Cache:
                _validator?.ValidateCacheStore(handle);

                await view.HideAsync(ct);
                await _cache.StoreAsync(type, handle);

                handle.State = UIHandleState.Cached;
                break;

            case UIReusePolicy.Release:
                await _presentation.DismissAsync(handle, ct);
                handle.State = UIHandleState.Released;
                break;
            case UIReusePolicy.Destroy:
            {
                await _presentation.DestroyAsync(handle, ct);
                handle.State = UIHandleState.Destroy;
                break;
            }    
        }
    }

    // =====================================================
    // RETENTION LIMIT
    // =====================================================

    private void EnforceRetentionLimit(Type viewType)
    {
        var entry = _manifest.Get(viewType);

        if (entry.ReusePolicy != UIReusePolicy.Retain)
            return;

        int count = 0;

        var node = _screenList.Last;

        while (node != null)
        {
            var handle = node.Value;

            if (handle.Instance.View.GetType() == viewType)
            {
                count++;

                if (count > entry.RetainDepth)
                {
                    handle.Instance.View.HideAsync(default).Forget();
                    handle.State = UIHandleState.Hidden;
                }
            }

            node = node.Previous;
        }
    }
    // =====================================================
    // ReplaceAsync
    // =====================================================
    public async UniTask<UIHandle> ReplaceAsync(Type viewType, CancellationToken ct)
    {
        UIHandle result;

        await _lock.WaitAsync(ct);

        try
        {
            if (_screenList.Count > 0)
            {
                var current = _screenList.Last.Value;
                _screenList.RemoveLast();

                await ProcessReusePolicy(
                    current,
                    _manifest.Get(current.Instance.View.GetType()),
                    ct);
            }
        }
        finally
        {
            _lock.Release();
        }
        result = await PushAsync(viewType, ct);

        return result;
    }
    // =====================================================
    // UTIL
    // =====================================================

    private void RemoveFromAllContainers(UIHandle handle)
    {
        RemoveFromList(_screenList, handle);
        RemoveFromList(_modalList, handle);

        var type = handle.Instance.View.GetType();

        if (_overlays.TryGetValue(type, out var existing) && existing == handle)
            _overlays.Remove(type);
    }

    private void RemoveFromList(
        LinkedList<UIHandle> list,
        UIHandle target)
    {
        var node = list.First;

        while (node != null)
        {
            if (node.Value == target)
            {
                list.Remove(node);
                return;
            }

            node = node.Next;
        }
    }
    private bool IsTopSame(Type viewType, EUILayer layer)
    {
        return layer switch
        {
            EUILayer.Screen => _screenList.Last?.Value?.Instance.View.GetType() == viewType,
            EUILayer.Popup => _modalList.Last?.Value?.Instance.View.GetType() == viewType,
            EUILayer.Overlay => _overlays.ContainsKey(viewType),
            _ => false
        };
    }
    // =====================================================
    // QUERY
    // =====================================================

    public UIHandle Peek()
    {
        return _screenList.Last?.Value ?? _modalList.Last?.Value;
    }

    public bool HasAny()
    {
        return _screenList.Count > 0
            || _modalList.Count > 0
            || _overlays.Count > 0;
    }
}

    
