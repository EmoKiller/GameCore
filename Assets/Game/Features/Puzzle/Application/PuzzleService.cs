using System;
using System.Collections.Generic;
using Game.Application.Core;
using UnityEngine;

public interface IPuzzleService : IService
{
    IReadOnlyPuzzleBoard Board { get; }
    SwapResult TrySwap( TilePosition a, TilePosition b);
    void GenerateBoard();

    event Action BoardChanged;
    event Action<CascadeResult> CascadeResolved;
    
    bool IsInside(TilePosition position);
}
public sealed class PuzzleService : IPuzzleService
{
    public IReadOnlyPuzzleBoard Board => _board;

    public EPuzzleState State => _state;
    private EPuzzleState _state;

    private readonly PuzzleBoard _board;

    private readonly BoardGenerator _generator;

    private readonly ISwapProcessor _swapProcessor;

    private readonly CascadeProcessor _cascadeProcessor;

    
    public event Action BoardChanged;

    public event Action<CascadeResult> CascadeResolved;

    public PuzzleService(
        PuzzleBoard board,
        BoardGenerator generator,
        ISwapProcessor swapProcessor,
        CascadeProcessor cascadeProcessor)
    {
        _board = board;

        _generator = generator;

        _swapProcessor = swapProcessor;

        _cascadeProcessor = cascadeProcessor;

        _state = EPuzzleState.Idle;
    }
    public void GenerateBoard()
    {
        _generator.Fill(_board);
        BoardChanged?.Invoke();
    }
    public SwapResult TrySwap(
        TilePosition a,
        TilePosition b)
    {
        if (_state != EPuzzleState.Idle)
        {
            return new SwapResult(
                false,
                null
            );
        }

        _state = EPuzzleState.Busy;

        var swapChangeSet = new BoardChangeSet();

        bool success = _swapProcessor.TrySwap(
                _board,
                a,
                b,
                swapChangeSet);
        
        if (success == false)
        {
            _state = EPuzzleState.Idle;

            return new SwapResult(
                false,
                null
                );
        }

        var swapContext = new SwapContext(a, b);

        var cascadeResult = _cascadeProcessor.Process(_board, swapChangeSet, swapContext);
        
        CascadeResolved?.Invoke(cascadeResult);
        
        _state = EPuzzleState.Idle;

        return new SwapResult(
            true,
            cascadeResult
        );
    }
    public bool IsInside(TilePosition position)
    {
        return _board.IsInside(position);
    }
}
