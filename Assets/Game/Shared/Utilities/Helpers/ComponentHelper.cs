using UnityEngine;

public static class ComponentHelper 
{
    public static T GetOrAddComponent<T>(GameObject obj) where T : Component
    {
        T component = obj.GetComponent<T>();

        if (component == null)
            component = obj.AddComponent<T>();

        return component;
    }        
}
