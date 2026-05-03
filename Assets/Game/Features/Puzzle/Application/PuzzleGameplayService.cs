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
public sealed class PuzzleGameplayService : IPuzzleGameplayService
{
    private readonly PuzzleBoardViewFactory _puzzleBoardViewFactory;
    private readonly IPuzzleService _puzzleService;
    private IReadOnlyBoardLayout _boardLayout;
    private PuzzleBoardAnimator _boardAnimator;
    private PuzzleBoardView _boardView;
    private PuzzleSessionService _sessionService;
    private bool _isBusy;
    public bool IsBusy => _isBusy;

    
    public PuzzleGameplayService(
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
    }
    public async UniTask CreateRuntime(CancellationToken ct)
    {
        _puzzleService.GenerateBoard();
        _boardView = await _puzzleBoardViewFactory.Create(_puzzleService, _boardLayout, ct);

        _boardAnimator.InitializeBoard(_boardView);
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



        if (result.Success)
        {
            foreach (CascadeStepResult StepResult in result.CascadeResult.Steps)
            {
                await _boardAnimator.PlayAsync(StepResult.ChangeSet);
            }

            //_sessionService.ProcessMove(result.CascadeResult);
        }
        else
        {
            await _boardAnimator.PlayInvalidSwapAsync(a, b);
        }
        _isBusy = false;
    }
    public bool IsInside(TilePosition position)
    {
        return _puzzleService.IsInside(position);
    }

}
