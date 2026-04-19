
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Application.UI.Core.Abstractions;
using Game.Presentation.UI;
using Game.Presentation.UI.Data;
using UnityEngine;

namespace UI.Core.Factory
{
    public interface IUIViewFactory
    {
        UniTask<IUIView> CreateAsync(Type viewType, CancellationToken ct);
    }
    public sealed class UIViewFactory
    {
        private readonly IAssetProvider _assetProvider;
        private readonly UIManifest _manifest;
        private readonly IUIRoot _root;

        public UIViewFactory(
            IAssetProvider assetProvider,
            UIManifest manifest,
            IUIRoot root)
        {
            _assetProvider = assetProvider;
            _manifest = manifest;
            _root = root;
        }

        public async UniTask<IUIView> CreateAsync(Type viewType, CancellationToken ct)
        {
            var entry = _manifest.Get(viewType);

            var prefab = await _assetProvider.LoadAsync<GameObject>(entry.AssetKey, ct);

            var go = UnityEngine.Object.Instantiate( prefab.Asset, _root.GetRoot(entry.Layer));

            var view = go.GetComponent(viewType);

            if (view == null)
                throw new Exception($"Missing view component: {viewType.Name}");

            return (IUIView)view;
        }
    }
}