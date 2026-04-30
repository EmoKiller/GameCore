using UnityEngine;
namespace Game.Application.Core
{
    public interface IUpdatable
    {
        void OnUpdate(float deltaTime);
    }
    public interface IFixedUpdatable
    {
        void OnFixedUpdatable(float fixedDeltaTime);
    }
    public interface ILateUpdatable
    {
        void OnLateUpdatable(float deltaTime);
    }
}