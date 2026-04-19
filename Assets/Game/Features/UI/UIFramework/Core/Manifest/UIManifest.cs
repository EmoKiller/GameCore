using System;
using System.Collections.Generic;
using System.Linq;
using Game.Presentation.UI.Data;

public sealed class UIManifest
{
    private readonly Dictionary<Type, UIManifestEntry> _map;
    public IEnumerable<UIManifestEntry> Entries => _map.Values;    

    public UIManifest(IEnumerable<UIManifestEntry> entries)
    {
        _map = entries.ToDictionary(e => e.ViewType, e => e);
    }

    public UIManifestEntry Get(Type viewType)
    {
        if (!_map.TryGetValue(viewType, out var entry))
            throw new Exception($"UI not registered: {viewType.Name}");

        return entry;
    }
}