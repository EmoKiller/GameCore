using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Application.Core.Input;
using UnityEngine;
public interface ICharacterRuntimeFactory
{
    UniTask<CharacterRuntime> CreateAsync(CharacterDefinition definition, CancellationToken ct);
}
public sealed class CharacterRuntimeFactory : ICharacterRuntimeFactory
{
    private readonly IAssetProvider _assets;
    private readonly IResourceFactory _resourceFactory;

    public CharacterRuntimeFactory(
        IAssetProvider assets,
        IResourceFactory resourceFactory)
    {
        _assets = assets;
        _resourceFactory = resourceFactory;
    }

    public async UniTask<CharacterRuntime> CreateAsync(
        CharacterDefinition def,
        CancellationToken ct)
    {
        // //1. Load View
        // var handle = await _assets.LoadAsync<GameObject>(def.ViewAssetKey, ct);
        // var view = GameObject.Instantiate(handle.Asset)
        //                      .GetComponent<ICharacterView>();

        // // 2. Stats
        // var statsFactory = new CharacterStatsFactory(_resourceFactory);
        // var stats = statsFactory.Create(def.StatsConfig);
        // var statsFacade = new CharacterStatsFacade(stats);

        // // 4. Context
        // var context = new PlayerContext(
        //     actions: view,
        //     stats: statsFacade,
        //     input: input,
        //     flipCharacter: new FlipCharacter2D(view.GetTransform())
        // );

        // // 5. StateMachine
        // var stateSystem = new PlayerStateSystem(context);

        // // 6. Presenter
        // var presenter = new PlayerPresenter(context, stateSystem);
        // presenter.SetView(view);

        return new CharacterRuntime();
    }
}
