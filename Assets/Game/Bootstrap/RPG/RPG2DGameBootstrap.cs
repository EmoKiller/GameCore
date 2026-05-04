using Game.Application.Core;
using Game.Application.Core.Input;
using Game.Application.Core.Modules;
using Game.Application.Modules.Assets;
using Game.Application.Modules.UIModules;
using UnityEngine;
namespace Game.Bootstrap
{
    public class RPG2DGameBootstrap : GameBootstrap
    {
        protected override void RegisterCoreServices()
        {
            base.RegisterCoreServices();
        }

        protected override void RegisterModules()
        {
            // var factory = new ModuleFactory();

            // // Core systems
            // factory.AddModule<EventModule>(App);
            // factory.AddModule<AssetModule>(App);
            // factory.AddModule<InputModule>(App);

            // // Presentation
            // factory.AddModule<UIModule>(App);
            // factory.AddModule<CemeraModule>(App);

            // // Gameplay
            // factory.AddModule<PlayerModule>(App);

            // // Flow
            // factory.AddModule<RPGGameFlowModule>(App);
        }
    }
}