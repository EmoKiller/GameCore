using Game.Application.Core;
using Game.Character.Core.Stats;

public interface IResourceSystem : IService
{
    void Initialize(CharacterStats stats, CharacterStatsConfig config);
    void Tick(float deltaTime);
}
public sealed class ResourceSystem : IResourceSystem
{
    private ResourceRuntimeData[] _data;
    private int _count;

    public void Initialize(CharacterStats stats, CharacterStatsConfig config)
    {
        _data = new ResourceRuntimeData[config.Resources.Count];
        _count = 0;

        foreach (var rc in config.Resources)
        {
            var resource = (Resource)stats.Read.GetResource(rc.Type);

            var runtime = new ResourceRuntimeData
            {
                Resource = resource,
                Config = rc,
                RegenDelay = 0f
            };

            var regenConfig = rc.Regen;

            resource.OnConsumed += _ =>
            {
                if (regenConfig != null)
                    runtime.RegenDelay = regenConfig.DelayAfterUse;
            };

            _data[_count++] = runtime;
        }
    }

    public void Tick(float dt)
    {
        for (int i = 0; i < _count; i++)
        {
            var data = _data[i];

            Process(data, dt);
        }
    }

    private static void Process(ResourceRuntimeData data, float dt)
    {
        var resource = data.Resource;
        var config = data.Config;

        if (data.RegenDelay > 0f)
        {
            data.RegenDelay -= dt;
            return;
        }

        if (!config.Behavior.HasFlag(EResourceBehaviorFlags.Regenerates))
            return;

        var regen = config.Regen;

        if (regen == null || regen.RatePerSecond <= 0f)
            return;

        resource.Restore(regen.RatePerSecond * dt);
    }
}