public sealed class ResourceRegenConfig
{
    public float RatePerSecond { get; }
    public float DelayAfterUse { get; }

    public ResourceRegenConfig(float ratePerSecond, float delayAfterUse)
    {
        RatePerSecond = ratePerSecond;
        DelayAfterUse = delayAfterUse;
    }
}