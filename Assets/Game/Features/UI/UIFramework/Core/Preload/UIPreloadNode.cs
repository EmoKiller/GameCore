using System;
using System.Collections.Generic;
using Game.Presentation.UI.View;

public interface IUIPreloadNode
{
    Type ViewType { get; }

    IReadOnlyList<Type> Dependencies { get; }

    bool PreloadOnStart { get; }
}

public abstract class UIPreloadNodeBase : IUIPreloadNode
{
    public abstract Type ViewType { get; }

    public virtual IReadOnlyList<Type> Dependencies => Array.Empty<Type>();

    public virtual bool PreloadOnStart => false;
}

public sealed class LoadingPreloadNode : UIPreloadNodeBase
{
    public override Type ViewType => typeof(LoadingView);

    public override bool PreloadOnStart => true;
}

public sealed class MainMenuPreloadNode : UIPreloadNodeBase
{
    public override Type ViewType => typeof(MainMenuScreen);

    public override IReadOnlyList<Type> Dependencies =>new[]
    {
        typeof(LoadingView)
    };
}