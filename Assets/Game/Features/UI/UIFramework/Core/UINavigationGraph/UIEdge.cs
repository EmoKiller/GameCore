using System;

public sealed class UIEdge
{
    public string Action;
    public Type TargetViewType;

    public bool ClearStack;
    public bool AllowBack;
    
    public IUINavigationGuard Guard;
}