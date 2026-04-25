using System.Collections.Generic;
using Game.Application.ReactiveProperty;

public sealed class LoadingContext
{
    public GameStateContext Game { get; }
    public LoadingProgressAggregator Progress { get; }

    public Dictionary<string, object> Items { get; } = new();

    public LoadingContext(GameStateContext game, LoadingProgressAggregator progress)
    {
        Game = game;
        Progress = progress;
    }

    public void Set<T>(T value)
    {
        Items[typeof(T).FullName] = value;
    }

    public T Get<T>()
    {
        return (T)Items[typeof(T).FullName];
    }
}