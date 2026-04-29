using Game.Application.Events;
using UnityEngine;

public sealed class EnsurePlayableUseCase
{
    private readonly IMoveFinder _moveFinder;
    private readonly IBoardGenerator _generator;
    private readonly IEventBus _eventBus;

    public EnsurePlayableUseCase(
        IMoveFinder moveFinder,
        IBoardGenerator generator,
        IEventBus eventBus)
    {
        _moveFinder = moveFinder;
        _generator = generator;
        _eventBus = eventBus;
    }

    public void Execute(IGrid grid)
    {
        if (_moveFinder.HasAnyMove(grid))
            return;

        _generator.Generate(grid);

        _eventBus.Publish(new BoardShuffledEvent(), EventChannel.System);
    }
}