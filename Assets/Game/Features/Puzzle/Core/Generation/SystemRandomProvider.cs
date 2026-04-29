using UnityEngine;
public interface IRandomProvider
{
    int Next(int min, int max);
}
public sealed class SystemRandomProvider : IRandomProvider
{
    private readonly System.Random _random = new();

    public int Next(int min, int max)
    {
        return _random.Next(min, max);
    }
}
