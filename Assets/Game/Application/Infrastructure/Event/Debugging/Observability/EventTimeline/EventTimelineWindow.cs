#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public sealed class EventTimelineWindow : EditorWindow
{
    [MenuItem("Tools/Observability/Event Timeline")]
    public static void Open()
    {
        GetWindow<EventTimelineWindow>("Event Timeline");
    }

    private void OnGUI()
    {
        GUILayout.Label("Event Timeline Profiler", EditorStyles.boldLabel);

        // TODO:
        // - timeline list
        // - filters
        // - graph view
    }
}
#endif


// EventBus integration

// Inside dispatch pipeline:

// traceCollector.Record(new EventTraceEntry
// {
//     EventName = eventName,
//     Type = TraceEventType.EventPublished,
//     Timestamp = Time.realtimeSinceStartupAsDouble
// });


// Flow integration
// tracer.OnStepStart(stepName);
// await step.ExecuteAsync(...)
// tracer.OnStepEnd(stepName, duration);