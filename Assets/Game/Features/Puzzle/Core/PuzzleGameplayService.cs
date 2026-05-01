using Cysharp.Threading.Tasks;
using Game.Application.Core;
using UnityEngine;
public interface IPuzzleGameplayService : IService
{
    void CreateRuntime();
    UniTask TrySwapAsync( TilePosition a, TilePosition b);
}
public sealed class PuzzleGameplayService : IPuzzleGameplayService
{
    private PuzzleBoardView _boardView;

    private PuzzleBoardViewFactory _puzzleBoardViewFactory;



    private IPuzzleService _puzzleService;

    private PuzzleBoardAnimator _animator;

    private bool _isBusy;
    public PuzzleGameplayService(IPuzzleService PuzzleService, PuzzleBoardViewFactory puzzleBoardViewFactory)
    {
        _puzzleService = PuzzleService;
        _puzzleBoardViewFactory = puzzleBoardViewFactory;
    }
    public void CreateRuntime()
    {
        _boardView = _puzzleBoardViewFactory.Create();

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

        var result =
            _puzzleService.TrySwap(a, b);

        if (result.Success == false)
        {
            _isBusy = false;
            return;
        }

        foreach (var changeSet in result.ChangeSets)
        {
            await _animator.PlayAsync(changeSet);
        }

        _isBusy = false;
    }
}
