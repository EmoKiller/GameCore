using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Application.Core;
using Game.Application.Core.Input;
using Game.Character.Core.Stats;
using UnityEngine;

public class PlayerModule : BaseGameModule
{
    public override string ModuleName => "PlayerModule";

    public override int InitializationOrder => 10;

    public override Type[] GetDependencies() => Type.EmptyTypes;

    protected override async UniTask OnInitializeAsync(IServiceContainer services, CancellationToken ct)
    {   
        var assetProvider = services.Resolve<IAssetProvider>();
        // Create GameObject Player 
        var playerGO = await assetProvider.LoadAsync<GameObject>("CharacterView", ct);
        var playerInput = services.Resolve<IInputService>();

        var playerFactory = new PlayerFactory(playerGO.Asset);

        var playerService = new PlayerService(
            playerFactory,
            playerInput
        );
 

        services.Register<IPlayerService>(playerService);

        await UniTask.CompletedTask;
    }
    public override void Shutdown()
    {
        
    }

    private CharacterStatsConfig CharacterConfig()
    {
        var config = new CharacterStatsConfig(
            stats: new[]
            {
                new StatConfig(EStatType.MaxHealth, 100),
                new StatConfig(EStatType.Attack, 10),
                new StatConfig(EStatType.Defense, 5),
                new StatConfig(EStatType.MoveSpeed, 5),
                new StatConfig(EStatType.JumpForce, 10),
            },
            resources: new[]
            {
                new ResourceConfig(
                    EResourceType.Health,
                    EStatType.MaxHealth,
                    1f,
                    EResourceBehaviorFlags.Regenerates,
                    new ResourceRegenConfig(5f, 2f)
                ),
                new ResourceConfig(
                    EResourceType.Mana,
                    EStatType.MaxHealth,
                    0.5f,
                    EResourceBehaviorFlags.Regenerates,
                    new ResourceRegenConfig(5f, 2f)
                )
            }

        );
        return config;
    }
}
