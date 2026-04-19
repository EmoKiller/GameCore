using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Application.Core;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Game.Application.Core.Input
{
    public class InputModule : BaseGameModule
    {
        public override string ModuleName => "InputModule";

        public override int InitializationOrder => 0;

        public override Type[] GetDependencies()=> Type.EmptyTypes;

        protected override async UniTask OnInitializeAsync(IServiceContainer services, CancellationToken ct)
        {
            var playerInput = GameObject.FindFirstObjectByType<PlayerInput>();
            
            if (playerInput == null)
                throw new Exception("PlayerInput not found in scene");

            var inputDeviceDetector = new InputDeviceDetector();      

            var inputService = new InputService(
                playerInput,
                inputDeviceDetector
                );

            services.Register<IInputService>(inputService);

            await UniTask.CompletedTask;
        }
        public override void Shutdown()
        {
            
        }

        
    }
}
