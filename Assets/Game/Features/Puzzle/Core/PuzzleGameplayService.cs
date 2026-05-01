using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Application.Core;
using UnityEngine;
public interface IPuzzleGameplayService : IService
{
    UniTask CreateRuntime(CancellationToken ct);
    UniTask TrySwapAsync( TilePosition a, TilePosition b);
    bool IsInside( TilePosition position);
    bool IsBusy { get; }
}
public sealed class PuzzleGameplayService : IPuzzleGameplayService
{
    private PuzzleBoardView _boardView;

    private PuzzleBoardViewFactory _puzzleBoardViewFactory;
    private IPuzzleService _puzzleService;

    private PuzzleBoardAnimator _animator;

    private bool _isBusy;
    public bool IsBusy => _isBusy;
    public PuzzleGameplayService(IPuzzleService PuzzleService, PuzzleBoardViewFactory puzzleBoardViewFactory)
    {
        _puzzleService = PuzzleService;
        _puzzleBoardViewFactory = puzzleBoardViewFactory;
    }
    public async UniTask CreateRuntime(CancellationToken ct)
    {
        _boardView = await _puzzleBoardViewFactory.Create(ct);

        _puzzleService.GenerateBoard();
        _boardView.InitializeBoard(_puzzleService, 1f);

        _animator =
            new PuzzleBoardAnimator(
                _boardView);
    }
    public async UniTask TrySwapAsync(
        TilePosition a,
        TilePosition b)
    {
        if (_isBusy)
        {
            return;
        }

        _isBusy = true;

        SwapResult result = _puzzleService.TrySwap(a, b);
        //Debug.Log( $"Swap Result: {result.Success}");
        //await _animator.PlayAsync(result.ChangeSets)

        if (result.Success)
        {
            foreach (BoardChangeSet changeSet in result.ChangeSets)
            {
                await _animator.PlayAsync(changeSet);
            }
        }

        _isBusy = false;
    }
    public bool IsInside(TilePosition position)
    {
        return _puzzleService.IsInside(position);
    }
}
