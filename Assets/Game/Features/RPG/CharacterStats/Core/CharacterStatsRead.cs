using System;

namespace Game.Character.Core.Stats
{
    public interface IReadOnlyCharacterStats
    {
        IReadOnlyStat GetStat(EStatType type);
        IReadOnlyResource GetResource(EResourceType type);

        bool TryGetStat(EStatType type, out IReadOnlyStat stat);
        bool TryGetResource(EResourceType type, out IReadOnlyResource resource);
    }
    public sealed class CharacterStatsRead : IReadOnlyCharacterStats
    {
        private readonly StatContainer _stats;
        private readonly ResourceContainer _resources;

        public CharacterStatsRead(
            StatContainer stats,
            ResourceContainer resources)
        {
            _stats = stats ?? throw new ArgumentNullException(nameof(stats));
            _resources = resources ?? throw new ArgumentNullException(nameof(resources));
        }

        // =========================
        // STAT ACCESS
        // =========================

        public IReadOnlyStat GetStat(EStatType type)
        {
            return _stats.Get(type);
        }

        public bool TryGetStat(EStatType type, out IReadOnlyStat stat)
        {
            if (_stats.TryGet(type, out var s))
            {
                stat = s; 
                return true;
            }

            stat = null;
            return false;
        }

        // =========================
        // RESOURCE ACCESS
        // =========================

        public IReadOnlyResource GetResource(EResourceType type)
        {
            return _resources.Get(type);
        }

        public bool TryGetResource(EResourceType type, out IReadOnlyResource resource)
        {
            if (_resources.TryGet(type, out var res))
            {
                resource = res;
                return true;
            }

            resource = null;
            return false;
        }
    }
}