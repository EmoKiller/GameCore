using Game.Application.Core.Input;
using UnityEngine;

public sealed class PlayerRuntime
{
    public ICharacterView View { get; }
    private readonly PlayerPresenter _presenter;
    private PlayerStateSystem _playerStateSystem;

    private PlayerRuntime(
        ICharacterView view,
        PlayerPresenter presenter)
    {
        View = view;
        _presenter = presenter;
    }

    // public static PlayerRuntime Create(
    //     ICharacterView view,
    //     IInputService inputService,
    //     ICharacterStatsFacade stats)
    // {
    //     var input = inputService.GetPlayerInput();
    //     input.Initialize();

    //     var context = new PlayerContext(
    //         actions: view, // giả định view implements actions
    //         stats: stats,
    //         input: input,
    //         flipCharacter: new FlipCharacter2D(view.GetTransform())
    //     );

    //     var stateSystem = new PlayerStateSystem(context);

    //     var presenter = new PlayerPresenter(context);
    //     presenter.SetView(view);

    //     return new PlayerRuntime(view, presenter);
    // }

    public void Update(float dt)
    {
        _presenter.Update(dt);
    }
}
