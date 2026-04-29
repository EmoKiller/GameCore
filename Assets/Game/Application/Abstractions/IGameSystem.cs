using Game.Application.Core;
using UnityEngine;

public interface IGameSystem
{
    int Order { get; }

    void Initialize(IServiceContainer services);
    
    void Tick(float dt);
    void FixedTick(float dt);
    void LateTick(float dt);
    void Shutdown();
}
