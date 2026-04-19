using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game.Application.Configuration.Abstractions;
using UnityEngine;

namespace Game.Application.Configuration.Runtime
{
    public sealed class ConfigLoader
    {
        private readonly IConfigProvider _provider;
        private readonly IConfigRegistry _registry;

        public ConfigLoader(
            IConfigProvider provider,
            IConfigRegistry registry)
        {
            _provider = provider;
            _registry = registry;
        }

        /// <summary>
        /// Load toàn bộ config của loại T từ Provider, Validate và nạp vào Registry.
        /// </summary>
        public async UniTask LoadAsync<T>(
            Func<T, int> idSelector, 
            IEnumerable<IConfigValidator<T>> validators = null) 
            where T : IConfig
        {
            try
            {
                // 1. Load dữ liệu từ Provider (Addressables, JSON, etc.)
                var result = await _provider.LoadConfigsAsync<T>();
                var configs = result.ToList();

                if (configs.Count == 0)
                {
                    Debug.LogWarning($"[ConfigLoader] No configs found for type: {typeof(T).Name}");
                    return;
                }

                // 2. Chạy Validation (nếu có)
                if (validators != null)
                {
                    foreach (var config in configs)
                    {
                        foreach (var validator in validators)
                        {
                            validator.Validate(config, configs);
                        }
                    }
                }

                // 3. Nạp vào Registry
                _registry.Register(configs, idSelector);
                
                Debug.Log($"[ConfigLoader] Successfully loaded {configs.Count} configs of type {typeof(T).Name}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[ConfigLoader] Failed to load configs of type {typeof(T).Name}: {e.Message}");
                throw; // Rethrow để Bootstrapper biết và xử lý (ví dụ hiện thông báo lỗi)
            }
        }
    }
}