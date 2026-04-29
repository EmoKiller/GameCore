using System.Collections.Generic;
using Game.Application.Events;

public interface IPuzzleSystem 
{
    void EnqueueSwap(SwapCommand command);
    IReadOnlyGrid Grid { get; }
}
public sealed class PuzzleSystem : IPuzzleSystem
{
    private readonly IGrid _grid;
    
    private readonly ISwapValidator _swapValidator;
    private readonly IMatchResolutionSystem _resolution;
    private readonly IDeadBoardDetector _deadDetector;
    private readonly IShuffleSystem _shuffle;
    private readonly IEventBus _events;

    private readonly Queue<SwapCommand> _queue = new();

    public IReadOnlyGrid Grid => _grid;

    public PuzzleSystem(
        IGrid grid,
        ISwapValidator swapValidator,
        IMatchResolutionSystem resolution,
        IDeadBoardDetector deadDetector,
        IShuffleSystem shuffle,
        IEventBus events)
    {
        _grid = grid;
        _swapValidator = swapValidator;
        _resolution = resolution;
        _deadDetector = deadDetector;
        _shuffle = shuffle;
        _events = events;
    }

    public void EnqueueSwap(SwapCommand command)
    {
        _queue.Enqueue(command);
    }

    public void Update(float deltaTime)
    {
        if (_queue.Count == 0)
            return;

        var command = _queue.Dequeue();

        ProcessSwap(command);
    }

    private void ProcessSwap(SwapCommand cmd)
    {
        if (!_swapValidator.CanSwap(_grid, cmd.X1, cmd.Y1, cmd.X2, cmd.Y2))
            return;

        _grid.Swap(cmd.X1, cmd.Y1, cmd.X2, cmd.Y2);

        _events.Publish(new SwapPerformedEvent(cmd) , EventChannel.Gameplay);

        var matches = _resolution.Resolve(_grid);

        if (matches.Count > 0)
        {
            _events.Publish(new MatchesResolvedEvent(matches), EventChannel.Gameplay);
        }

        if (_deadDetector.IsDead(_grid))
        {
            _shuffle.Shuffle(_grid);
            _events.Publish(new BoardShuffledEvent(), EventChannel.Gameplay);
        }
    }
}