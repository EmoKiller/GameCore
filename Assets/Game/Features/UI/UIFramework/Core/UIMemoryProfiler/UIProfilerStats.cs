public sealed class UIProfilerStats
{
    public int Created;
    public int Reused;

    public int Released;
    public int Destroyed;

    public float HitRate =>
        (Created + Reused) == 0
            ? 0f
            : (float)Reused / (Created + Reused);

    public void Record(UIProfilerEvent evt)
    {
        switch (evt.Type)
        {
            case UIProfilerEventType.Create:
                Created++;
                break;

            case UIProfilerEventType.ReuseFromPool:
                Reused++;
                break;

            case UIProfilerEventType.ReleaseToPool:
                Released++;
                break;

            case UIProfilerEventType.Destroy:
                Destroyed++;
                break;
        }
    }
}
