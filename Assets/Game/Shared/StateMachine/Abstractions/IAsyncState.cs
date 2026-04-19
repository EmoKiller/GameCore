

using System.Threading;
using Cysharp.Threading.Tasks;
namespace Game.Share.StateMachine
{
    public interface IAsyncState<TContext>
    {
        UniTask EnterAsync(TContext context, CancellationToken ct);
        UniTask ExitAsync(TContext context, CancellationToken ct);
    }

}
