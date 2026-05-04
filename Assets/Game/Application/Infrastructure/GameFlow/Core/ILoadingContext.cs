using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface ILoadingStateContext
{
    EGameState NextGameState { get; }
    bool IsLoadingCompleted { get; }
    UniTask ShowLoading(CancellationToken ct);
    UniTask HideLoading(CancellationToken ct);

    void MarkLoadingAsCompleted();
}
