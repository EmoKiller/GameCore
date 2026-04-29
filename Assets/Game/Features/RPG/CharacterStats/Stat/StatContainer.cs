using System;
using System.Collections;
using System.Collections.Generic;

namespace Game.Character.Core.Stats
{
    public sealed class StatContainer
    {
        private readonly Dictionary<EStatType, StatValue> _stats = new();

        public void Add(EStatType type, StatValue stat)
        {
            _stats[type] = stat;
        }

        public StatValue Get(EStatType type)
        {
            return _stats[type];
        }

        public bool TryGet(EStatType type, out StatValue stat)
        {
            return _stats.TryGetValue(type, out stat);
        }

        public IEnumerable<StatValue> GetAll()
        {
            return _stats.Values;
        }
    }
}