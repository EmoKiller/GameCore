using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
public interface IUINavigator
{
    UniTask NavigateAsync(string action, CancellationToken ct);
    UniTask GoBackAsync(CancellationToken ct);
}
public sealed class UINavigator : IUINavigator
{
    private readonly UIFlowGraph _graph;
    private readonly IUIRouter _router;
    private readonly IUINavigationHistory _history;

    private readonly UINavigationContext _context = new();

    private Type _current;
    public bool HasCurrent => _current != null;

    public UINavigator(
        UIFlowGraph graph,
        IUIRouter router,
        IUINavigationHistory history)
    {
        _graph = graph;
        _router = router;
        _history = history;
    }

    public async UniTask NavigateAsync(string action, CancellationToken externalCt)
    {
        if (_current == null)
            throw new Exception("No current UI. Call ShowAsync first.");

        _context.CancelCurrent();

        using var linkedCts =
            CancellationTokenSource.CreateLinkedTokenSource(
                _context.Token,
                externalCt);

        var ct = linkedCts.Token;

        var node = _graph.GetNode(_current);

        var edge = node.Edges
            .FirstOrDefault(e => e.Action == action);

        if (edge == null)
            throw new Exception($"No route: {action}");

        // ===== GUARD =====
        if (edge.Guard != null)
        {
            var can = await edge.Guard
                .CanNavigateAsync(_current, edge.TargetViewType, ct);

            if (!can)
                return;
        }

        // ===== HISTORY =====
        if (!edge.ClearStack && _current != null)
        {
            _history.Push(new UINavigationEntry(_current, null));
        }
        else
        {
            _history.Clear();
        }

        // ===== ROUTER ONLY =====
        await _router.ReplaceAsync(edge.TargetViewType, ct);

        _current = edge.TargetViewType;
    }

    public async UniTask GoBackAsync(CancellationToken externalCt)
    {
        // 1. Không có history → bỏ
        if (!_history.TryPop(out var entry))
            return;

        // 2. cancel flow cũ
        _context.CancelCurrent();

        using var linkedCts =
            CancellationTokenSource.CreateLinkedTokenSource(
                _context.Token,
                externalCt);

        var ct = linkedCts.Token;

        try
        {
            // 3. remove current UI
            await _router.PopTopAsync(ct);

            // 4. push lại previous
            var handle = await _router.PushAsync(entry.ViewType, ct);

            _current = entry.ViewType;

            // =========================
            // 5. RESTORE STATE (✔ ĐÚNG CHỖ DUY NHẤT)
            // =========================
            if (entry.State != null &&
                handle.Instance.View is IUIStateful stateful)
            {
                stateful.RestoreState(entry.State);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[UI BACK ERROR] {ex}");
        }
    }

    public void SetInitial(Type viewType)
    {
        if (_current != null)
            return;
        _current = viewType;
    }

}
