using System.Collections;
using System.Collections.Generic;

namespace Game.Character.Core.Stats
{
    public class ResourceContainer : IEnumerable<Resource>
    {
        private readonly Dictionary<EResourceType, Resource> _resources = new();

        public void Add(EResourceType type, Resource resource)
        {
            _resources[type] = resource;
        }

        public T Get<T>(EResourceType type) where T : Resource
        {
            return (T)_resources[type];
        }
        
        public Resource Get(EResourceType type)
        {
            return _resources[type];
        }

        public bool TryGet(EResourceType type, out Resource resource)
        {
            return _resources.TryGetValue(type, out resource);
        }

        public bool Has(EResourceType type)
        {
            return _resources.ContainsKey(type);
        }

        public IEnumerator<Resource> GetEnumerator()
        {
            return _resources.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
    
    
    
}