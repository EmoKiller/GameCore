using Game.Application.Core;
using UnityEngine;

public interface IRandomProvider : IService
{
    int Range(int minInclusive, int maxExclusive);
}
public sealed class UnityRandomProvider : IRandomProvider
{
    public int Range(int minInclusive, int maxExclusive)
    {
        return UnityEngine.Random.Range( minInclusive, maxExclusive);
    }
}