

using System.Threading;
using Cysharp.Threading.Tasks;
namespace Game.Share.StateMachine
{

    public interface IState<TContext> 
    {
        void Enter(TContext owner);
        void Exit(TContext owner);
    }
    public interface IUpdateState<TContext>
    {
        void Update(TContext context, float deltaTime);
    }
    public interface IFixedUpdateState<TContext>
    {
        void FixedUpdate(TContext context, float fixedDeltaTime);
    }
    

}
