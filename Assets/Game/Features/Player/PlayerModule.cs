using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Application.Core;
using UnityEngine;

public class PlayerModule : BaseGameModule
{
    public override string ModuleName => "PlayerModule";

    public override int InitializationOrder => 10;

    public override Type[] GetDependencies() => Type.EmptyTypes;
    /// <summary>
    /// Config chứa các thông số cơ bản của Character như max health,
    /// move speed, được thiết lập trong Inspector và sử dụng để khởi tạo model.
    /// </summary>
    [SerializeField] protected CharacterStatsConfig config;

    protected override async UniTask OnInitializeAsync(IServiceContainer services, CancellationToken ct)
    {
        // Create GameObject Player 
        var playerGO = await services.Resolve<IAssetProvider>().LoadAsync<GameObject>("Player", ct);


        var playerFactory = new PlayerFactory(playerGO.Asset);
        var playerService = new PlayerService(playerFactory);
 

        services.Register<IPlayerService>(playerService);

        await UniTask.CompletedTask;
    }
    public override void Shutdown()
    {
        
    }

    
}
