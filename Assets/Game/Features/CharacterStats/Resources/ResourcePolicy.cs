using System;
using UnityEngine;
public interface IResourcePolicy
{
    float Clamp(float value, float max);
    bool IsDepleted(float current);
}
public sealed class DefaultResourcePolicy : IResourcePolicy
{
    public float Clamp(float value, float max)
    {
        return Math.Clamp(value, 0f, max);
    }

    public bool IsDepleted(float current)
    {
        return current <= 0f;
    }
}
public sealed class OvercapResourcePolicy : IResourcePolicy
{
    public float Clamp(float value, float max)
    {
        return Math.Max(0f, value); 
    }

    public bool IsDepleted(float current)
    {
        return current <= 0f;
    }
}
public sealed class NonDepletablePolicy : IResourcePolicy
{
    public float Clamp(float value, float max)
    {
        return Math.Clamp(value, 0f, max);
    }

    public bool IsDepleted(float current)
    {
        return false;
    }
}
