using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Game.Application.Configuration.Abstractions;
using System;
using Cysharp.Threading.Tasks;

namespace Game.Application.Configuration.Providers
{
    public sealed class JsonConfigProvider : IConfigProvider
    {
        private readonly string _folderPath;

        public JsonConfigProvider(string folderPath)
        {
            _folderPath = folderPath;
        }

        public async UniTask<IEnumerable<T>> LoadConfigsAsync<T>() where T : IConfig
        {
            var path = Path.Combine(_folderPath, $"{typeof(T).Name}.json");

            if (!File.Exists(path))
                return new List<T>();

            var json = await File.ReadAllTextAsync(path);

            var list = JsonConvert.DeserializeObject<List<T>>(json);

            return list ?? new List<T>();
        }
    }
}
