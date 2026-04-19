using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Application.Core;
using Game.Application.UI.Core.Abstractions;

public interface IUIService : IService
{
    UniTask ShowAsync<TView>(CancellationToken ct = default)
        where TView : class, IUIView;

    UniTask HideAsync<TView>(CancellationToken ct = default)
        where TView : class, IUIView;

    UniTask NavigateAsync(string action, CancellationToken ct = default);

    UniTask BackAsync(CancellationToken ct = default);

    UniTask ResetAsync(CancellationToken ct = default);
}

public sealed class UIService : IUIService
{
    private readonly UINavigator _navigator;
    private readonly IUIRouter _router;

    public UIService( UINavigator navigator, IUIRouter router)
    {
        _navigator = navigator;
        _router = router;
    }

    public async UniTask ShowAsync<TView>(CancellationToken ct = default)
        where TView : class, IUIView
    {
        var type = typeof(TView);

        await _router.PushAsync(type, ct);

        if (!_navigator.HasCurrent)
            _navigator.SetInitial(type);
    }

    public UniTask HideAsync<TView>(CancellationToken ct = default)
        where TView : class, IUIView
    {
        return _router.PopAsync(typeof(TView), ct);
    }

    public UniTask NavigateAsync(string action, CancellationToken ct = default)
    {
        return _navigator.NavigateAsync(action, ct);
    }

    public UniTask BackAsync(CancellationToken ct = default)
    {
        return _navigator.GoBackAsync(ct);
    }

    public async UniTask ResetAsync(CancellationToken ct = default)
    {
        while (_router.HasAny())
        {
            await _router.PopTopAsync(ct);
        }
    }

}
