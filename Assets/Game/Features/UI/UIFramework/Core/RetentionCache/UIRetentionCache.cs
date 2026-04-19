using UnityEngine;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;

public interface IUIRetentionCache
{
    UniTask StoreAsync(Type viewType, UIHandle handle);

    bool TryGet(Type viewType, out UIHandle handle);

    void Remove(Type viewType);

    UniTask TrimAsync(int maxCount);

}

public sealed class UIRetentionCache : IUIRetentionCache
{
    private readonly Dictionary<Type, UIHandle> _cache = new();
    private readonly LinkedList<Type> _lru = new();
    private readonly IUIPresentationService _presentation;
    private readonly IUIRuntimeValidator _validator;

    private const int MAX_CACHE = 5;

    public UIRetentionCache(IUIPresentationService presentation , IUIRuntimeValidator validator)
    {
        _presentation = presentation;
        _validator = validator;
    }
    
    public async UniTask StoreAsync(Type viewType, UIHandle handle)
    {
        if (_cache.ContainsKey(viewType))
            return;

        _cache[viewType] = handle;
        _lru.AddLast(viewType);
        await TrimAsync(MAX_CACHE);
        _validator.ValidateCacheStore(handle);
    }

    public bool TryGet(Type viewType, out UIHandle handle)
    {
        if (_cache.TryGetValue(viewType, out handle))
        {
            _lru.Remove(viewType);
            _lru.AddLast(viewType);
            _validator.ValidateCacheRetrieve(handle);
            return true;
        }

        return false;
    }

    public void Remove(Type viewType)
    {
        if (!_cache.ContainsKey(viewType))
            return;

        _cache.Remove(viewType);
        _lru.Remove(viewType);
    }

    public async UniTask TrimAsync(int maxCount)
    {
        while (_cache.Count > maxCount)
        {
            var oldest = _lru.First.Value;

            var handle = _cache[oldest];

            await _presentation.DismissAsync(handle, CancellationToken.None);
            _cache.Remove(oldest);
            _lru.Remove(oldest);
        }
    }

}
