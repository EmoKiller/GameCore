using UnityEngine;
namespace Game.Character.Core.Stats
{
    public interface ICharacterStatsFactory
    {
        CharacterStats Create(CharacterStatsConfig config);
    }

    public sealed class CharacterStatsFactory : ICharacterStatsFactory
    {
        private readonly IResourceFactory _resourceFactory;

        public CharacterStatsFactory(IResourceFactory resourceFactory)
        {
            _resourceFactory = resourceFactory;
        }
        public CharacterStats Create(CharacterStatsConfig config)
        {
            CharacterStatsConfigValidator.Validate(config);

            var stats = new StatContainer();
            var resources = new ResourceContainer();

            // =========================
            // CREATE STATS
            // =========================
            foreach (var s in config.Stats)
            {
                var stat = new StatValue(s.BaseValue);
                stats.Add(s.Type, stat);
            }

            // =========================
            // CREATE RESOURCES
            // =========================
            foreach (var r in config.Resources)
            {
                var stat = stats.Get(r.MaxStatType);

                var resource = _resourceFactory.Create(r, stat);

                resources.Add(r.Type, resource);
            }

            return new CharacterStats(
                new CharacterStatsRead(stats, resources),
                new CharacterStatsWriter(stats, resources)
            );
        }

        // =========================
        // POLICY MAPPING (CRITICAL)
        // =========================
        private static IResourcePolicy CreatePolicy(ResourceConfig config)
        {
            var behavior = config.Behavior;

            if (behavior.HasFlag(EResourceBehaviorFlags.AllowOvercap))
            {
                return new OvercapResourcePolicy();
            }

            if (!behavior.HasFlag(EResourceBehaviorFlags.CanBeDepleted))
            {
                return new NonDepletablePolicy();
            }

            return new DefaultResourcePolicy();
        }
    }
}
