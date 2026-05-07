public abstract class TileRuntimeSpecialState
{
    public ESpecialLifecycleState LifecycleState;
}
public sealed class NomalRuntimeSpecialState : TileRuntimeSpecialState
{
    public NomalRuntimeSpecialState(ESpecialLifecycleState lifecycleState)
    {
        LifecycleState = lifecycleState;
    }
} 
public sealed class AreaClearRuntimeState : TileRuntimeSpecialState
{
    public int RemainingCharges;
    

    public AreaClearRuntimeState(ESpecialLifecycleState lifecycleState = ESpecialLifecycleState.None, int remainingCharges = 0)
    {
        RemainingCharges = remainingCharges;
        LifecycleState = lifecycleState;
    }
    public AreaClearRuntimeState( int remainingCharges = 0, ESpecialLifecycleState lifecycleState = ESpecialLifecycleState.None)
    {
        RemainingCharges = remainingCharges;
        LifecycleState = lifecycleState;
    }
}