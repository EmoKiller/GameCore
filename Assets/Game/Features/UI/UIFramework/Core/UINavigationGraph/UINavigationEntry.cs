using System;

public sealed class UINavigationEntry
{
    public Type ViewType { get; }
    public object State { get; }

    public UINavigationEntry(Type viewType, object state)
    {
        ViewType = viewType;
        State = state;
    }

}
