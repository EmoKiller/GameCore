using System.Collections.Generic;
using UnityEngine;
using Game.Application.Configuration.Abstractions;
using Cysharp.Threading.Tasks;
using System;

namespace Game.Application.Configuration.Providers
{
    public sealed class ScriptableObjectConfigProvider : IConfigProvider
    {
        private readonly IEnumerable<ScriptableObject> _assets;

        public ScriptableObjectConfigProvider(IEnumerable<ScriptableObject> assets)
        {
            _assets = assets ?? throw new ArgumentNullException(nameof(assets));
        }

        /// <summary>
        /// Load ScriptableObject configs asynchronously.
        /// Không block main thread, trả về UniTask<IEnumerable<T>>.
        /// </summary>
        public UniTask<IEnumerable<T>> LoadConfigsAsync<T>() where T : IConfig
        {
            var configs = new List<T>();

            foreach (var asset in _assets)
            {
                if (asset is T config)
                    configs.Add(config);
            }

            return UniTask.FromResult<IEnumerable<T>>(configs);
        }
    }
}