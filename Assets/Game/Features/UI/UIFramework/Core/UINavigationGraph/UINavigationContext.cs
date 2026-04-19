using System.Threading;

public sealed class UINavigationContext
{
    private CancellationTokenSource _cts;

    public CancellationToken Token => _cts?.Token ?? CancellationToken.None;

    public void CancelCurrent()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = new CancellationTokenSource();
    }
}