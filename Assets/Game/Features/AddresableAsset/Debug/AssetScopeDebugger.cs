using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
public class ScopeInfo
{
    public int Id { get; }
    public string Name { get; }
    public HashSet<object> Assets { get; }

    public ScopeInfo(int id, string name)
    {
        Id = id;
        Name = name;
        Assets = new HashSet<object>();
    }
}
public static class AssetScopeDebugger
{
    private static readonly Dictionary<int, ScopeInfo> _scopes = new();
    private static readonly Dictionary<object, int> _assetToScope = new();

    public static void RegisterScope(int id, string name)
    {
        _scopes[id] = new ScopeInfo(id, name);
    }

    public static void UnregisterScope(int id)
    {
        if (_scopes.TryGetValue(id, out var scope))
        {
            foreach (var asset in scope.Assets)
            {
                _assetToScope.Remove(asset);
            }
        }

        _scopes.Remove(id);
    }

    public static void Track(int scopeId, object asset, string key)
    {
        if (!_scopes.TryGetValue(scopeId, out var scope))
            return;

        // detect double ownership
        if (_assetToScope.TryGetValue(asset, out var existingScope))
        {
            Debug.LogWarning(
                $"[ScopeDebugger] Asset '{key}' already tracked by scope {existingScope}, now added to {scopeId}");
        }

        scope.Assets.Add(asset);
        _assetToScope[asset] = scopeId;
    }

    public static void Untrack(int scopeId, object asset)
    {
        if (_scopes.TryGetValue(scopeId, out var scope))
        {
            scope.Assets.Remove(asset);
        }

        _assetToScope.Remove(asset);
    }

    public static void Dump()
    {
        Debug.Log("===== Scope Debug Dump =====");

        foreach (var scope in _scopes.Values)
        {
            Debug.Log($"Scope [{scope.Id}] {scope.Name} - Assets: {scope.Assets.Count}");
        }
    }
}
