using Game.Application.Core.Modules;
using Game.Application.Modules.Assets;
using Game.Application.Modules.UIModules;

namespace Game.Bootstrap
{
    public class PuzzleGameBootstrap : GameBootstrap
    {
        protected override void RegisterModules()
        {
            var factory = new ModuleFactory();

            factory.AddModule<AssetModule>(App);
            factory.AddModule<UIModule>(App);


            factory.AddModule<PuzzleModule>(App);
            factory.AddModule<PuzzleGameplayModule>(App);
            factory.AddModule<PuzzleInputModule>(App);
            

            factory.AddModule<PuzzleGameFlowModule>(App);
        }
    }
}