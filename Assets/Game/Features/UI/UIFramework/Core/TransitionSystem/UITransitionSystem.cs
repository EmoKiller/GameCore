using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
public interface IUITransitionSystem
{
    UniTask EnterAsync(UIHandle handle, CancellationToken ct);
    UniTask ExitAsync(UIHandle handle, CancellationToken ct);
}
public sealed class UITransitionSystem : IUITransitionSystem
{
    public async UniTask EnterAsync(UIHandle handle, CancellationToken ct)
    {
        var view = handle.Instance.View;

        await view.ShowAsync(ct);

        var transition = GetTransition(handle, true);
        if (transition != null)
            await transition.PlayAsync(view, ct);
    }

    public async UniTask ExitAsync(UIHandle handle, CancellationToken ct)
    {
        var view = handle.Instance.View;

        var transition = GetTransition(handle, false);
        if (transition != null)
            await transition.PlayAsync(view, ct);

        await view.HideAsync(ct);
    }

    private IUITransition GetTransition(UIHandle handle, bool enter)
    {
        // can be resolved from manifest or node config
        return null;
    }
}