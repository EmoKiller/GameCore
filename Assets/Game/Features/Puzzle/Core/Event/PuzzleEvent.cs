using System.Collections.Generic;
using Game.Application.Events;
using UnityEngine;
public readonly struct SwapPerformedEvent : IEvent
{
    public SwapCommand Command { get; }

    public SwapPerformedEvent(SwapCommand command)
    {
        Command = command;
    }
}
public readonly struct MatchesResolvedEvent : IEvent
{
    public IReadOnlyList<Match> Matches { get; }

    public MatchesResolvedEvent(IReadOnlyList<Match> matches)
    {
        Matches = matches;
    }
}
public readonly struct BoardShuffledEvent : IEvent { }