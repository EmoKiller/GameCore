using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Game.Application.Core;
using UnityEngine;

public class PuzzleModule : BaseGameModule
{
    public override string ModuleName => "PuzzleModule";

    public override int InitializationOrder => 10;

    public override Type[] GetDependencies() => Type.EmptyTypes;

    

    protected override async UniTask OnInitializeAsync(IServiceContainer services, CancellationToken ct)
    {

        await UniTask.CompletedTask;
    }
    public override void Shutdown()
    {
        
    }
}
