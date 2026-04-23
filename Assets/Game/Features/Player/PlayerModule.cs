using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Application.Core;
using Game.Application.Core.Input;
using UnityEngine;

public class PlayerModule : BaseGameModule
{
    public override string ModuleName => "PlayerModule";

    public override int InitializationOrder => 10;

    public override Type[] GetDependencies() => Type.EmptyTypes;
    /// <summary>
    /// Config chứa các thông số cơ bản của Character như max health,
    /// move speed, được thiết lập trong Inspector và sử dụng để khởi tạo model.

    protected override async UniTask OnInitializeAsync(IServiceContainer services, CancellationToken ct)
    {   
        var assetProvider = services.Resolve<IAssetProvider>();
        // Create GameObject Player 
        var playerGO = await assetProvider.LoadAsync<GameObject>("Player", ct);
        var playerInput = services.Resolve<IInputService>();
        var handleConfig = await assetProvider.LoadAsync<CharacterStatsConfig>("PlayerConfig", ct);

        var playerFactory = new PlayerFactory(playerGO.Asset);

        var playerService = new PlayerService(
            playerFactory,
            playerInput,
            handleConfig.Asset
        );
 

        services.Register<IPlayerService>(playerService);

        await UniTask.CompletedTask;
    }
    public override void Shutdown()
    {
        
    }

    
}
