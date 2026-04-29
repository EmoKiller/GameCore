using Game.Application.Events;

public sealed class SwapUseCase
{
    private readonly ISwapSimulator _simulator;
    private readonly IEventBus _eventBus;

    public SwapUseCase(
        ISwapSimulator simulator,
        IEventBus eventBus)
    {
        _simulator = simulator;
        _eventBus = eventBus;
    }

    public bool Execute(IGrid grid, in SwapCommand command)
    {
        if (!_simulator.WouldCreateMatch(
            grid,
            command.X1, command.Y1,
            command.X2, command.Y2))
        {
            return false;
        }

        var t1 = grid.Get(command.X1, command.Y1);
        var t2 = grid.Get(command.X2, command.Y2);

        grid.Set(command.X1, command.Y1, t2);
        grid.Set(command.X2, command.Y2, t1);

        _eventBus.Publish(new SwapPerformedEvent(command) , EventChannel.System);

        return true;
    }
}