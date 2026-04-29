using Game.Application.Events;

public sealed class PuzzlePresenter : IEventHandler<IEvent>
{
    public int Priority => 0;
    public EventChannel Channel => EventChannel.System;

    private readonly IPuzzleSystem _system;
    private readonly BoardView _view;

    public PuzzlePresenter(IPuzzleSystem system, BoardView view)
    {
        _system = system;
        _view = view;

        _view.Initialize(system.Grid);
    }

    public void Handle(IEvent evt)
    {
        switch (evt)
        {
            case SwapPerformedEvent:
            case MatchesResolvedEvent:
            case BoardShuffledEvent:
                _view.Render();
                break;
        }
    }
}