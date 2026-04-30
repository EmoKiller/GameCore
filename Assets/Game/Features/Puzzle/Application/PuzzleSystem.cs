using System.Collections.Generic;
using Game.Application.Core;
using Game.Application.Events;

public interface IPuzzleSystem : IService
{
    IReadOnlyGrid Grid { get; }

    void Initialize(int width, int height);

    void ExecuteSwap(in SwapCommand command);
}
public sealed class PuzzleSystem : IPuzzleSystem
{
    private readonly InitializeBoardUseCase _init;
    private readonly SwapUseCase _swap;
    private readonly ResolveMatchesUseCase _resolve;
    private readonly EnsurePlayableUseCase _ensure;
    
    private IGrid _grid;

    public IReadOnlyGrid Grid => _grid;

    public PuzzleSystem(
        InitializeBoardUseCase init,
        SwapUseCase swap,
        ResolveMatchesUseCase resolve,
        EnsurePlayableUseCase ensure)
    {
        _init = init;
        _swap = swap;
        _resolve = resolve;
        _ensure = ensure;
    }

    public void Initialize(int width, int height)
    {
        _grid = new Grid(width, height);

        _init.Execute(_grid);
        _ensure.Execute(_grid);
    }

    public void ExecuteSwap(in SwapCommand command)
    {
        if (!_swap.Execute(_grid, command))
            return;

        _resolve.Execute(_grid);
        _ensure.Execute(_grid);
    }
}