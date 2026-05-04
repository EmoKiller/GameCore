using System;
using Game.Application.UI.Core.Abstractions;
namespace Game.Presentation.UI.Data
{
    [Serializable]
    public sealed class UIManifestEntry
    {
        public string Id;

        public string AssetKey;

        public Type ViewType;
        public Type ViewModelType;
        public Type PresenterType;

        public EUILayer Layer;

        
        public UIReusePolicy ReusePolicy;
        public int CachePriority;

        // ✔ RETENTION CONTROL
        public int RetainDepth = 1;

        // ✔ POOL CONTROL
        public int PoolWarmupSize = 1;
        public int PoolCapacity = 1;

        
        
    }
}