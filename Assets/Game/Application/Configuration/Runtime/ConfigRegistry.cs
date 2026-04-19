using System;
using System.Collections.Generic;
using Game.Application.Configuration.Abstractions;
namespace Game.Application.Configuration.Runtime
{
    public sealed class ConfigRegistry : IConfigRegistry
    {
        // Dictionary lưu trữ: Type -> Dictionary<int, T>
        private readonly Dictionary<Type, object> _configs = new();

        public void Register<T>(IEnumerable<T> configs, Func<T, int> idSelector) where T : IConfig
        {
            var type = typeof(T);
            var dict = new Dictionary<int, T>();

            foreach (var config in configs)
            {
                dict[idSelector(config)] = config;
            }

            _configs[type] = dict;
        }

        public void Append<T>(IEnumerable<T> configs, Func<T, int> idSelector) where T : IConfig
        {
            var dict = GetOrAddDict<T>();
            foreach (var config in configs)
            {
                dict[idSelector(config)] = config;
            }
        }

        public void Unregister<T>() where T : IConfig
        {
            _configs.Remove(typeof(T));
        }

        public void ClearAll()
        {
            _configs.Clear();
        }

        public T Get<T>(int id) where T : IConfig
        {
            if (TryGetDict<T>(out var dict) && dict.TryGetValue(id, out var config))
            {
                return config;
            }

            throw new KeyNotFoundException($"[ConfigRegistry] Config {typeof(T).Name} with ID {id} not found.");
        }

        public bool TryGet<T>(int id, out T config) where T : IConfig
        {
            config = default;
            return TryGetDict<T>(out var dict) && dict.TryGetValue(id, out config);
        }

        public IReadOnlyCollection<T> GetAll<T>() where T : IConfig
        {
            return TryGetDict<T>(out var dict) ? dict.Values : Array.Empty<T>();
        }

        public bool IsRegistered<T>() where T : IConfig => _configs.ContainsKey(typeof(T));

        public bool Contains<T>(int id) where T : IConfig 
            => TryGetDict<T>(out var dict) && dict.ContainsKey(id);

        public int GetCount<T>() where T : IConfig 
            => TryGetDict<T>(out var dict) ? dict.Count : 0;

        // --- HELPER METHODS ---

        private bool TryGetDict<T>(out Dictionary<int, T> dict) where T : IConfig
        {
            if (_configs.TryGetValue(typeof(T), out var obj))
            {
                dict = (Dictionary<int, T>)obj;
                return true;
            }
            dict = null;
            return false;
        }

        private Dictionary<int, T> GetOrAddDict<T>() where T : IConfig
        {
            var type = typeof(T);
            if (!_configs.TryGetValue(type, out var obj))
            {
                obj = new Dictionary<int, T>();
                _configs[type] = obj;
            }
            return (Dictionary<int, T>)obj;
        }









        // private readonly Dictionary<Type, object> _configs = new();

        // public void Register<T>(IEnumerable<T> configs, Func<T, ConfigId<T>> idSelector) where T : IConfig
        // {
        //     var dict = new Dictionary<ConfigId<T>, T>();

        //     foreach (var config in configs)
        //     {
        //         dict[idSelector(config)] = config;
        //     }
        //     if (_configs.ContainsKey(typeof(T)))
        //         throw new InvalidOperationException($"Config already registered: {typeof(T)}");
        //     _configs[typeof(T)] = dict;
        // }

        // public T Get<T>(ConfigId<T> id) where T : IConfig
        // {
        //     var dict = GetDict<T>();

        //     if (dict.TryGetValue(id, out var config))
        //         return config;

        //     throw new KeyNotFoundException($"Config not found: {id}");
        // }

        // public bool TryGet<T>(ConfigId<T> id, out T config) where T : IConfig
        // {
        //     var dict = GetDict<T>();
        //     return dict.TryGetValue(id, out config);
        // }

        // public IReadOnlyCollection<T> GetAll<T>() where T : IConfig
        // {
        //     return GetDict<T>().Values;
        // }

        // private Dictionary<ConfigId<T>, T> GetDict<T>() where T : IConfig
        // {
        //     if (!_configs.TryGetValue(typeof(T), out var obj))
        //         throw new InvalidOperationException($"Config type not registered: {typeof(T)}");

        //     return (Dictionary<ConfigId<T>, T>)obj;
        // }
    }
}