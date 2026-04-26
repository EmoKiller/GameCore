using Cysharp.Threading.Tasks;
using Game.Application.Core;
using Game.Application.Core.Input;
using Game.Character.Core;
using Game.Character.Core.Stats;
using UnityEngine;
public interface IPlayerService : IService
{
    Transform GetTransform();
    void Spawn();
    
}
public sealed class PlayerService : IPlayerService, IUpdatable
{
    private readonly PlayerFactory _factory;
    private readonly IInputService _input;
    private readonly ICharacterStatsFacade _stats;

    private PlayerRuntime _runtime;

    public PlayerService(
        PlayerFactory factory,
        IInputService input,
        ICharacterStatsFacade stats)
    {
        _factory = factory;
        _input = input;
        _stats = stats;
    }

    public void Spawn()
    {
        // var view = _factory.Create();

        // _runtime = PlayerRuntime.Create(
        //     view,
        //     _input,
        //     _stats
        // );

        GameApplication.Instance.Lifecycle.Register(this);
    }

    public Transform GetTransform()
    {
        return _runtime.View.GetTransform();
    }

    public void OnUpdate(float dt)
    {
        _runtime.Update(dt);
    }
}
