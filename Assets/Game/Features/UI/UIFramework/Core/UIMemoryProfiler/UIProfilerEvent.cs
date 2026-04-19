using System;

public readonly struct UIProfilerEvent
{
    public readonly UIProfilerEventType Type;
    public readonly Type ViewType;
    public readonly float Time;

    public UIProfilerEvent(
        UIProfilerEventType type,
        Type viewType,
        float time)
    {
        Type = type;
        ViewType = viewType;
        Time = time;
    }

}
