using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
public interface IUIPresentationService
{
    UniTask<UIHandle> PresentAsync(Type viewType, CancellationToken ct);

    UniTask DismissAsync(UIHandle handle, CancellationToken ct);
    UniTask DestroyAsync(UIHandle handle, CancellationToken ct);
}
public sealed class UIPresentationService : IUIPresentationService
{
    private readonly IUIPreloadSystem _preload;
    private readonly IUIViewPool _pool;
    private readonly IUICompositionService _composer;
    private readonly IUITransitionSystem _transition;
    private readonly UIManifest _manifest;

    private readonly IUIProfilerService _profiler;
    private readonly IUIRuntimeValidator _validator;

    public UIPresentationService(
        IUIPreloadSystem preload,
        IUIViewPool pool,
        IUICompositionService composer,
        IUITransitionSystem transition,
        UIManifest manifest,
        IUIProfilerService profiler,
        IUIRuntimeValidator validator)
    {
        _preload = preload;
        _pool = pool;
        _composer = composer;
        _transition = transition;
        _manifest = manifest;
        _profiler = profiler;
        _validator = validator;
    }

    // =====================================================
    // PRESENT (CREATE + ENTER)
    // =====================================================

    public async UniTask<UIHandle> PresentAsync( Type viewType, CancellationToken ct)
    {
        
        var entry = _manifest.Get(viewType);

        await _preload.PreloadAsync(viewType,entry.PoolWarmupSize, ct);

        var view = await _pool.GetAsync(viewType, ct) ?? throw new InvalidOperationException($"Failed to create view: {viewType.Name}");

        //  TẠO CompositionScope 
        var scope  = new UICompositionScope();

        //  PASS vào composition
        var instance = _composer.Compose(viewType, view, scope );

        var handle = new UIHandle(instance, scope );

        await _transition.EnterAsync(handle, ct);

        return handle;

    }

    // =====================================================
    // DISMISS (EXIT + RELEASE)
    // =====================================================
    public async UniTask DismissAsync( UIHandle handle, CancellationToken ct)
    {
        if (handle == null)
            return;
        if (handle.State == UIHandleState.Released)
            return;

        var instance = handle.Instance
            ?? throw new InvalidOperationException("UIHandle.Instance is null");

        var view = instance.View
            ?? throw new InvalidOperationException("UIView is null");

        var viewType = view.GetType();

        var entry = _manifest.Get(viewType);
        var type = entry.ViewType;

        var willReuse = entry.ReusePolicy == UIReusePolicy.Cache ||
                        entry.ReusePolicy == UIReusePolicy.Release;

        if (willReuse && view is not IReusableView)
        {
            throw new InvalidOperationException(
                $"{viewType.Name} must implement IReusableView");
        }

        // ================================
        // 1. EXIT TRANSITION
        // ================================

        _validator.ValidateBeforeDispose(handle);
        
        await _transition.ExitAsync(handle, ct);

        // ================================
        // 2. CLEANUP (NO POLICY HERE)
        // ================================

        // reset view (safe for pool reuse)
        if (view is IReusableView reusable)
            reusable.OnBeforeReuse();

        // dispose VM + Presenter
        // handle.Instance.ViewModel.Dispose();
        // handle.Instance.Presenter.Dispose();

        handle.CompositionScope.Dispose();

        _validator.ValidateBeforeDispose(handle);

        // ================================
        // 3. RELEASE TO POOL
        // ================================
        _pool.Release(type, view);

        _profiler.Record(new UIProfilerEvent(
            UIProfilerEventType.ReleaseToPool,
            viewType,
            Time.time));

    }
    public async UniTask DestroyAsync(UIHandle handle, CancellationToken ct)
    {
        if (handle == null)
            return;

        var instance = handle.Instance;
        var view = instance.View;
        var viewType = view.GetType();

        // 1. exit animation (optional)
        await _transition.ExitAsync(handle, ct);

        // 2. cleanup logic
        handle.CompositionScope.Dispose();

        // 3. destroy thật
        UnityEngine.Object.Destroy(((Component)view).gameObject);

        _profiler.Record(new UIProfilerEvent(
            UIProfilerEventType.Destroy,
            viewType,
            Time.time));
    }
}
