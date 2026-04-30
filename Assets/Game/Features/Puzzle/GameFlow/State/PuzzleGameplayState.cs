using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Share.StateMachine;
using UnityEngine;

public class PuzzleGameplayState : IAsyncState<PuzzleGameStateContext>
{
    private BoardView _boardView;
    private PuzzlePresenter _puzzlePresenter;
    public async UniTask EnterAsync(PuzzleGameStateContext context, CancellationToken ct)
    {
        var handle = await context.AssetProvider.LoadAsync<GameObject>("BoardView", ct);
        _boardView = GameObject.Instantiate(handle.Asset).GetComponent<BoardView>();
        context.PuzzleSystem.Initialize(8,8);
        _boardView.Initialize(context.PuzzleSystem.Grid);
        _boardView.RenderFull();

        var boardAnimationController = new BoardAnimationController();
        //var animationOrchestrator = new PuzzleAnimationOrchestrator(_boardView,boardAnimationController);
        _puzzlePresenter = new PuzzlePresenter(_boardView);

        await UniTask.CompletedTask;
    }

    public async UniTask ExitAsync(PuzzleGameStateContext context, CancellationToken ct)
    {
        await UniTask.CompletedTask;
    }

    
}
