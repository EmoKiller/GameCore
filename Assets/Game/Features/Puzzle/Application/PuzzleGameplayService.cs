using System.Collections;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Application.Core;
using UnityEngine;
public interface IPuzzleGameplayService : IService
{
    UniTask CreateRuntime(CancellationToken ct);
    UniTask TrySwapAsync( TilePosition a, TilePosition b);
    bool IsInside(TilePosition position);
    bool IsBusy { get; }
}
public sealed class PuzzleGameplayService : MonoBehaviour, IPuzzleGameplayService
{
    private PuzzleBoardViewFactory _puzzleBoardViewFactory;
    private IPuzzleService _puzzleService;
    private IReadOnlyBoardLayout _boardLayout;
    private PuzzleBoardAnimator _boardAnimator;
    private PuzzleBoardView _boardView;
    private PuzzleSessionService _sessionService;
    private bool _isBusy;
    public bool IsBusy => _isBusy;

    private CancellationTokenSource _runtimeCts;

    
    public void Initialized(
        IPuzzleService PuzzleService,
        PuzzleBoardViewFactory puzzleBoardViewFactory,
        PuzzleBoardAnimator boardAnimator,
        IReadOnlyBoardLayout boardLayout
    )
    {
        _puzzleService = PuzzleService;
        _puzzleBoardViewFactory = puzzleBoardViewFactory;
        _boardAnimator = boardAnimator;
        _boardLayout = boardLayout;
        ResetRuntimeToken();
    }
    public void ReloadBoard(BoardPreset preset)
    {
        ResetRuntimeToken();
        _puzzleService.LoadPreset(
            preset);

        _boardView.Rebuild(
            _puzzleService.Board);
    }
    public async UniTask CreateRuntime(CancellationToken ct)
    {
        _puzzleService.GenerateBoard();
        _boardView = await _puzzleBoardViewFactory.Create(_puzzleService, _boardLayout, ct);

        _boardAnimator.InitializeBoard(_boardView);
    }
    public async UniTask TrySwapAsync(TilePosition a, TilePosition b)
    {
        if (_isBusy)
        {
            return;
        }
        CancellationToken ct = _runtimeCts.Token;
        _isBusy = true;
        try
        {
            ct.ThrowIfCancellationRequested();

            SwapResult result =  _puzzleService.TrySwap(a, b);

            if (result.Success)
            {
                foreach (CascadeStepResult stepResult in result.CascadeResult.Steps)
                {
                    ct.ThrowIfCancellationRequested();

                    await _boardAnimator.PlayAsync(
                        stepResult.ChangeSet,
                        ct);
                }
            }
            else
            {
                await _boardAnimator.PlayInvalidSwapAsync(
                        a,
                        b,
                        ct);
            }
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            _isBusy = false;
        }
    }
    public bool IsInside(TilePosition position)
    {
        return _puzzleService.Board.IsInside(position);
    }
    private void ResetRuntimeToken()
    {
        _runtimeCts?.Cancel();

        _runtimeCts?.Dispose();

        _runtimeCts =
            new CancellationTokenSource();
    }
    void OnDestroy()
    {
        _runtimeCts?.Cancel();

        _runtimeCts?.Dispose();
    }
}
