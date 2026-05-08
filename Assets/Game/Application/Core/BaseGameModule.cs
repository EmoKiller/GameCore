using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Application.Core
{
    public abstract class BaseGameModule : MonoBehaviour, IGameModule , IAsyncInitializable
    {
        private string _cachedModuleName;

        public virtual string ModuleName 
        {
            get 
            {
                if (string.IsNullOrEmpty(_cachedModuleName))
                {
                    _cachedModuleName = this.GetType().Name;
                }
                return _cachedModuleName;
            }
        }
        public abstract Type[] GetDependencies();

        public async UniTask InitializeAsync(IServiceContainer services, CancellationToken ct = default)
        {
            try 
            {
                ct.ThrowIfCancellationRequested();
                await OnInitializeAsync(services, ct);
            }
            catch (OperationCanceledException)
            {
                Debug.LogWarning($"[Module] {ModuleName} initialization was canceled.");
                throw; // Ném ngược ra để phía gọi (Caller) biết là task đã dừng
            }
            catch (Exception e)
            {
                Debug.LogError($"[Module Error] {ModuleName} init failed: {e.Message}");
                throw; 
            }
        }

        protected abstract UniTask OnInitializeAsync(IServiceContainer services, CancellationToken ct);

        public abstract void Shutdown();

        
        
    }
}


