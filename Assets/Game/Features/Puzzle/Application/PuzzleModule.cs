using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Application.Core;
using UnityEngine;

public class PuzzleModule : IGameModule
{
    public string ModuleName => "PuzzleModule";

    public int InitializationOrder => 10;

    public Type[] GetDependencies()
    {
        return new Type[]
        {
            

        };
    }

    public UniTask InitializeAsync(IServiceContainer services, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public void Shutdown()
    {
        
    }
}
