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

        public bool IsInitialized { get; protected set; }

        public abstract Type[] GetDependencies();

        public async UniTask InitializeAsync(IServiceContainer services, CancellationToken ct = default)
        {
            if (IsInitialized) 
                return;

            try 
            {
                ct.ThrowIfCancellationRequested();
                await OnInitializeAsync(services, ct);
                ct.ThrowIfCancellationRequested();
                IsInitialized = true;
            }
            catch (OperationCanceledException)
            {
                // Log dạng Warning hoặc im lặng vì đây là hành vi bình thường
                Debug.LogWarning($"[Module] {ModuleName} initialization was canceled.");
                throw; // Ném ngược ra để phía gọi (Caller) biết là task đã dừng
            }
            catch (Exception e)
            {
                Debug.LogError($"[Module Error] {ModuleName} init failed: {e.Message}");
                throw; 
            }
        }

        // Các module con bắt buộc phải thực thi hàm này
        protected abstract UniTask OnInitializeAsync(IServiceContainer services, CancellationToken ct);

        public abstract void Shutdown();

        
        
    }
}


