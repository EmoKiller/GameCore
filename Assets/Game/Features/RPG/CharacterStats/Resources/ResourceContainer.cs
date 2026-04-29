using System;
using System.Collections;
using System.Collections.Generic;

namespace Game.Character.Core.Stats
{
    
    public class ResourceContainer : IDisposable
    {
        private readonly Dictionary<EResourceType, Resource> _resources = new();

        public void Add(EResourceType type, Resource resource)
        {
            _resources[type] = resource;
        }

        public Resource Get(EResourceType type)
        {
            if (!_resources.TryGetValue(type, out var res))
                throw new Exception($"Resource not found: {type}");

            return res;
        }

        public bool TryGet(EResourceType type, out Resource resource)
        {
            return _resources.TryGetValue(type, out resource);
        }

        public bool Has(EResourceType type)
        {
            return _resources.ContainsKey(type);
        }
        public void Dispose()
        {
            foreach (var r in _resources.Values)
            {
                r.Dispose();
            }
        }
    }
    
}