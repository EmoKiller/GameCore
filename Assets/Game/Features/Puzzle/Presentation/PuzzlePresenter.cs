using Cysharp.Threading.Tasks;
using Game.Application.Events;

public sealed class PuzzlePresenter : IEventHandler<IEvent>
{
    public int Priority => 0;
    public EventChannel Channel => EventChannel.System;

    private readonly BoardView _view;
    //private readonly PuzzleAnimationOrchestrator _orchestrator;
    public PuzzlePresenter(BoardView view)//, PuzzleAnimationOrchestrator orchestrator)
    {
        _view = view;
        //_orchestrator = orchestrator;
    }

    public void Handle(IEvent evt)
    {
        switch (evt)
        {
            case SwapPerformedEvent e:
                //HandleSwapAsync(e).Forget();
                break;
            case MatchesResolvedEvent:
            case BoardShuffledEvent:
                _view.RenderFull();
                break;
        }
    }
    // private async UniTaskVoid HandleSwapAsync(
    //     SwapPerformedEvent e)
    // {
    //     await _orchestrator.PlaySwapAsync(e.Command);
    // }
}