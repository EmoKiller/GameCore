using System.Collections;

using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Application.UI.Core.Abstractions
{
    public interface IUIView
    {
        UniTask ShowAsync(CancellationToken ct);
        UniTask HideAsync(CancellationToken ct);
    }
}
