using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Application.UI.Core.Abstractions;
using UnityEngine;

public interface IUITransition
{
    UniTask PlayAsync(IUIView view, CancellationToken ct);
}

public sealed class FadeTransition : IUITransition
{
    private readonly CanvasGroup _canvas;
    private readonly float _from;
    private readonly float _to;
    private readonly float _duration;

    public FadeTransition(CanvasGroup canvas, float from, float to, float duration)
    {
        _canvas = canvas;
        _from = from;
        _to = to;
        _duration = duration;
    }

    public async UniTask PlayAsync(IUIView view, CancellationToken ct)
    {
        float t = 0;

        _canvas.alpha = _from;

        while (t < _duration)
        {
            ct.ThrowIfCancellationRequested();

            t += Time.deltaTime;
            _canvas.alpha = Mathf.Lerp(_from, _to, t / _duration);

            await UniTask.Yield();
        }

        _canvas.alpha = _to;
    }
}