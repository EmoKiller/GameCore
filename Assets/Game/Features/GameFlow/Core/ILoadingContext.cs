using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface ILoadingContext
{
    EGameState NextGameState { get; }
    bool IsLoadingCompleted { get; }
    UniTask ShowLoading(CancellationToken ct);
    UniTask HideLoading(CancellationToken ct);

    void MarkLoadingAsCompleted();
}
