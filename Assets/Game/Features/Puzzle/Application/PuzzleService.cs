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
    void LoadPreset(BoardPreset preset);
}
public sealed class PuzzleService : MonoBehaviour, IPuzzleService
{
    public IReadOnlyPuzzleBoard Board => _board;

    public EPuzzleState State => _state;
    private EPuzzleState _state;

    private BoardPreset _debugPreset;

    private PuzzleBoard _board;

    private BoardGenerator _generator;

    private ISwapProcessor _swapProcessor;

    private CascadeProcessor _cascadeProcessor;

    
    public event Action BoardChanged;

    public event Action<CascadeResult> CascadeResolved;

    public void Initialized(
        PuzzleBoard board,
        BoardGenerator generator,
        ISwapProcessor swapProcessor,
        CascadeProcessor cascadeProcessor
    )
    {
        _board = board;

        _generator = generator;

        _swapProcessor = swapProcessor;

        _cascadeProcessor = cascadeProcessor;

        _state = EPuzzleState.Idle;
    }
    public void LoadPreset(BoardPreset preset)
    {
        _board = BoardPresetBuilder.Build(preset);
    }
    public void GenerateBoard()
    {
        if (_debugPreset != null)
        {
            _board = BoardPresetBuilder.Build(_debugPreset);
        }
        else
        {
            _generator.Fill(_board);
        }
        
        BoardChanged?.Invoke();
    }
    public SwapResult TrySwap(TilePosition a, TilePosition b)
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
