using UnityEngine;

public static class InterfaceHelper
{
    public static T RequireInterface<T>(MonoBehaviour source) where T : class
    {
        T result = source as T;

        if (result == null)
            Debug.LogError($"{source.name} does not implement {typeof(T)}");

        return result;
    }
}
