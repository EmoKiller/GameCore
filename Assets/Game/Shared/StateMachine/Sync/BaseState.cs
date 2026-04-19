
namespace Game.Share.StateMachine
{
    public abstract class BaseState<TContext> : IState<TContext>
    {
        public void Enter(TContext owner)
        {
            OnEnter(owner);
        }
        public void Exit(TContext owner)
        {
            OnExit(owner);
        }
        protected virtual void OnEnter(TContext owner){}
        protected virtual void OnExit(TContext owner){}
    }
    public abstract class BaseStateUpdate<TContext> : BaseState<TContext> , IUpdateState<TContext>
    {

        public void Update(TContext owner, float dt)
        {
            OnUpdate(owner, dt);
        }

        protected virtual void OnUpdate(TContext owner,float dt) { }
    }
    public abstract class BaseStateUpdateAndFixUpdate<TContext> : BaseStateUpdate<TContext> , IFixedUpdateState<TContext>
    {
        public void FixedUpdate(TContext context, float fixedDeltaTime)
        {
            OnFixUpdate(context, fixedDeltaTime);
        }
        protected virtual void OnFixUpdate(TContext context,float fixedDeltaTime) {}
    }
}




