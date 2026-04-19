using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Application.Core
{
    public interface IInitializable
    {
        UniTask InitializeAsync(CancellationToken ct);
    }
}
