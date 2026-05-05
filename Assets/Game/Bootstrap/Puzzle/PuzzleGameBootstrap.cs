using Game.Application.Core.Modules;
using Game.Application.Modules.Assets;
using Game.Application.Modules.UIModules;

namespace Game.Bootstrap
{
    public class PuzzleGameBootstrap : GameBootstrap
    {
        protected override void RegisterModules()
        {
            App.RegisterModule<AssetModule>();
            App.RegisterModule<UIModule>();
            App.RegisterModule<PuzzleModule>();
            App.RegisterModule<PuzzleGameplayModule>();
            App.RegisterModule<PuzzleInputModule>();
            App.RegisterModule<PuzzleGameFlowModule>();
        }
    }
}