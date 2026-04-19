using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Application.Core
{
    public abstract class BaseGameModule : IGameModule
    {
        public abstract string ModuleName { get; }

        public abstract int InitializationOrder { get; }

        public bool IsInitialized { get; protected set; }

        public abstract Type[] GetDependencies();

        public async UniTask InitializeAsync(IServiceContainer services, CancellationToken ct = default)
        {
            if (IsInitialized) 
                return;

            try 
            {
                await OnInitializeAsync(services, ct);
                IsInitialized = true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[Module Error] {ModuleName} init failed: {e.Message}");
                throw; // Re-throw để ModuleLoader bắt được
            }
        }

        // Các module con bắt buộc phải thực thi hàm này
        protected abstract UniTask OnInitializeAsync(IServiceContainer services, CancellationToken ct);

        public abstract void Shutdown();

        
        
    }
}


