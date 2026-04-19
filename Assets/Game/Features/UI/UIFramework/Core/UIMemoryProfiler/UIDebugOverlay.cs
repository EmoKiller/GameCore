using UnityEngine;
using System.Text;

public sealed class UIDebugOverlay : MonoBehaviour
{
    private IUIProfilerService _profiler;
    private readonly StringBuilder _sb = new();

    public void Inject(IUIProfilerService profiler)
    {
        _profiler = profiler;
    }

    private void OnGUI()
    {
        if (_profiler == null)
            return;

        var stats = _profiler.GetStats();

        _sb.Clear();

        _sb.AppendLine("=== UI PROFILER ===");

        foreach (var pair in stats)
        {
            var type = pair.Key;
            var s = pair.Value;

            _sb.AppendLine($"{type.Name}");
            _sb.AppendLine($" Create: {s.Created}");
            _sb.AppendLine($" Reuse : {s.Reused}");
            _sb.AppendLine($" Release: {s.Released}");
            _sb.AppendLine($" Destroy: {s.Destroyed}");
            _sb.AppendLine();
        }

        GUI.Label(new Rect(10, 10, 400, 800), _sb.ToString());
    }

}
