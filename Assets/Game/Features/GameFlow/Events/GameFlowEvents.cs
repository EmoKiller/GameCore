

using Game.Application.Events;

public readonly struct RequestStateChangeEvent : IEvent
{
    public readonly EGameState Target;

    public RequestStateChangeEvent(EGameState target)
    {
        Target = target;
    }
}

public readonly struct LoadingProgressEvent : IEvent
{
    public float Progress { get; }

    public LoadingProgressEvent(float progress)
    {
        Progress = progress;
    }
}
public struct LoadingCompletedEvent : IEvent { }