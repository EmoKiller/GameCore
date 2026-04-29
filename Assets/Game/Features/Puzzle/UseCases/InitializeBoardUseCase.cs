using Game.Application.Events;
using UnityEngine;

public sealed class InitializeBoardUseCase
{
    private readonly IBoardGenerator _generator;
    private readonly IEventBus _eventBus;

    public InitializeBoardUseCase(
        IBoardGenerator generator,
        IEventBus eventBus)
    {
        _generator = generator;
        _eventBus = eventBus;
    }

    public void Execute(IGrid grid)
    {
        _generator.Generate(grid);

        _eventBus.Publish(new BoardShuffledEvent() , EventChannel.System);
    }
}
