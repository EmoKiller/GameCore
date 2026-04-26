using Game.Character.Core.Stats;
public interface IResourceFactory
{
    Resource Create(ResourceConfig config, StatValue stat);
}
public sealed class ResourceFactory : IResourceFactory
{
    public Resource Create(ResourceConfig config, StatValue stat)
    {
        var policy = CreatePolicy(config);

        var resource = new Resource(stat, policy);

        resource.SetCurrent(stat.FinalValue * config.InitialValueRatio);

        return resource;
    }

    private static IResourcePolicy CreatePolicy(ResourceConfig config)
    {
        if (config.Behavior.HasFlag(EResourceBehaviorFlags.AllowOvercap))
            return new OvercapResourcePolicy();

        if (!config.Behavior.HasFlag(EResourceBehaviorFlags.CanBeDepleted))
            return new NonDepletablePolicy();

        return new DefaultResourcePolicy();
    }
}