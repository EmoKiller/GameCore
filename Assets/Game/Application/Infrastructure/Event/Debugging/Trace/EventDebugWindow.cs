#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Game.Application.Events.Debugging;

public class EventDebugWindow : EditorWindow
{
    private IEventTraceService _service;

    [MenuItem("Tools/Event Debug")]
    public static void Open()
    {
        GetWindow<EventDebugWindow>();
    }

    private void OnGUI()
    {
        if (_service == null)
        {
            GUILayout.Label("No trace service");
            return;
        }

        var traces = _service.GetTraces();

        foreach (var trace in traces)
        {
            GUILayout.Label(
                $"{trace.EventName} | {trace.Channel} | {trace.Duration}ms");

            foreach (var h in trace.Handlers)
            {
                GUILayout.Label($"  - {h.HandlerName}: {h.Duration}ms");
            }
        }
    }
}
#endif