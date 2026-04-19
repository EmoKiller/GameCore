using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Application.Core;
using Mono.Cecil.Cil;
using UnityEngine;

namespace Game.Application.Modules.Assets
{
    /// <summary>
    /// Module quản lý lifecycle asset
    /// </summary>
    public class AssetModule : BaseGameModule
    {
        public override string ModuleName => "AssetModule";
        public override int InitializationOrder => 0;
        public override Type[] GetDependencies() => Type.EmptyTypes;
        
        private readonly AssetScope _globalScope = new();

        private IAssetProvider _assetProvider;
        

        protected override async UniTask OnInitializeAsync(IServiceContainer services, CancellationToken ct)
        {   
            // Register Addressable Asset Provider
            _assetProvider = new AddressableAssetProvider(); 
            services.Register<IAssetProvider>(_assetProvider);

            await _assetProvider.InitializeAsync(ct);

            var handle = await _assetProvider.LoadAllByLabelAsync("staticglobal",ct);

            foreach (var h  in handle)
            {
                _globalScope.Track(h);
            }


            Debug.Log($"[{ModuleName}] System is fully ready.");

            // 3. LOAD CÁC TÀI NGUYÊN CƠ BẢN (Mở đầu/Global)
            // Ví dụ: Load các Label bắt buộc phải có để Game chạy được (Config, Atlas dùng chung...)
            // await assetProvider.LoadByLabelAsync<TextAsset>("GlobalConfig", ct);
            // await assetProvider.LoadByLabelAsync<SpriteAtlas>("MainUI_Atlas", ct);

            Debug.Log($"[{ModuleName}] Core assets loaded. Ready for other modules.");

        }

        public override void Shutdown()
        {
        }
    }
}

/// note
/// ===================================
/// 2 LoadAsync<T>(string key, CancellationToken ct)
/// Mục đích: Load một asset cụ thể (texture, prefab, audio clip…) theo key.
/// 
/// var bulletPrefab = await _assetProvider.LoadAsync<GameObject>("Bullet", ct);
/// 
/// Lưu ý: Dùng cho tải từng asset riêng lẻ, đặc biệt các prefab, scriptable object, audio, v.v.
/// ===================================
/// 3 LoadAsync(Type type, string key, CancellationToken ct)
/// Mục đích: Load asset dynamic type, khi bạn không biết type tại compile time.
/// 
/// Type Config = someConfig.Type;
/// var asset = await _assetProvider.LoadAsync(Config, "EnemyPrefab", ct);
/// 
/// Useful cho config-driven systems, ví dụ spawn enemy dựa trên JSON config.
///  ===================================
/// *** 4 LoadByLabelAsync<T>(string label, CancellationToken ct)
/// Load tập hợp asset có cùng label.
/// 
/// var musicTracks = await _assetProvider.LoadByLabelAsync<AudioClip>("Level1Music", ct);
/// 
/// Hoặc load danh sách prefab enemy theo label để spawn level.
/// Lưu ý: Mỗi asset vẫn được cache, nên gọi nhiều lần sẽ không duplicate.
///  ===================================
/// 5 InstantiateAsync(string key, Transform parent, AssetScope scope, CancellationToken ct)
/// Mục đích: Instantiate prefab Addressable vào scene, trả về InstanceHandle.
/// 
/// scope optional, để track lifetime → tự động release khi level unload.
///  ===================================
/// *** 6 PreloadAsync(IEnumerable<PreloadOperation> operations, AssetScope scope, CancellationToken ct)
/// Mục đích: Load trước nhiều asset, theo batch để giảm hitching runtime.
/// await _assetProvider.PreloadAsync(new[]
/// {
///     new PreloadOperation(typeof(GameObject), "EnemyPrefab"),
///     new PreloadOperation(typeof(AudioClip), "BGM_Level1")
/// }, levelScope, ct);
/// 
/// Tất cả asset sẽ được cache, ready để dùng khi gameplay.
///  ===================================
/// 7 Release<T>(AssetHandle<T> handle) và ReleaseAll()
/// Mục đích: Giải phóng asset khỏi cache và Addressables để giảm memory.
/// 
/// Release<T>: khi asset không còn cần dùng nữa:
/// _assetProvider.Release(enemyHandle);
/// 
/// ReleaseAll: khi unload level, scene, hoặc reset game:
/// _assetProvider.ReleaseAll();
/// 
///  ===================================
/// ** TryGetLoaded<T>(string key, out AssetHandle<T> handle)
/// Mục đích: Kiểm tra asset đã load chưa, tránh load lại.
/// Khi dùng: Khi muốn dùng asset đã có sẵn mà không trigger load async:
/// if (_assetProvider.TryGetLoaded<GameObject>("Bullet", out var handle))
/// {
///     var bullet = handle.Asset; // dùng ngay
/// }
/// else
/// {
///     // Chỉ khi không có mới load
///     var bullet = await _assetProvider.LoadAsync<GameObject>("Bullet", ct);
/// }
/// Tránh async / await nếu không cần:
/// Trong nhiều tình huống runtime performance-sensitive (spawn hàng trăm object/frame), bạn không muốn await mà chỉ muốn lấy asset đã có.
/// Debug / monitoring:
/// Muốn log hoặc kiểm tra cache hiện tại mà không thay đổi state của hệ thống.
/// 
/// 
/// 
/// 
/// 
/// 
