using Game.Application.Core.Modules;
using Game.Application.Modules.Assets;

namespace Game.Bootstrap
{
    public class PuzzleGameBootstrap : GameBootstrap
    {
        protected override void RegisterModules()
        {
            var factory = new ModuleFactory();

            factory.AddModule<AssetModule>(App);
            
            factory.AddModule<PuzzleModule>(App);




        }
    }
}