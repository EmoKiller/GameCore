using Game.Presentation.Events;

public abstract class MultiEventBinder : MonoEventBinder
{
    protected override void Bind()
    {
        RegisterHandlers(true);
    }

    protected override void Unbind()
    {
        RegisterHandlers(false);
    }

    protected abstract void RegisterHandlers(bool bind);
}