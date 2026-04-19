using System;
using UnityEngine;

[Serializable]
public sealed class SerializableType
{
    [SerializeField] private string _assemblyQualifiedName;

    private Type _cachedType;

    public Type Type
    {
        get
        {
            if (_cachedType == null && !string.IsNullOrEmpty(_assemblyQualifiedName))
            {
                _cachedType = Type.GetType(_assemblyQualifiedName);
            }
            return _cachedType;
        }
    }

    public void Set(Type type)
    {
        _cachedType = type;
        _assemblyQualifiedName = type?.AssemblyQualifiedName;
    }

}
