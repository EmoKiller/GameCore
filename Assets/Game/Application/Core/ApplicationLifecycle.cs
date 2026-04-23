using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Application.Core
{
    /// <summary>
    /// Triển khai trình phát sự kiện vòng đời ứng dụng.
    /// 
    /// Dịch vụ này công bố các sự kiện tại những thời điểm quan trọng trong vòng đời của ứng dụng.,
    /// cho phép các mô-đun và dịch vụ phản ứng mà không cần phụ thuộc trực tiếp.
    /// 
    /// Mô hình: Mô hình quan sát dựa trên sự kiện.
    /// Cách sử dụng: Đăng ký nhận thông báo về các sự kiện quan trọng đối với hệ thống của bạn.
    /// </summary>
    public class ApplicationLifecycle : IApplicationLifecycle
    {
        private ApplicationState _currentState = ApplicationState.PreInitialization;
        public ApplicationState CurrentState => _currentState;

        // Các danh sách Subscribers
        private readonly List<IOnPreInitialize> _onPreInitializes = new();
        private readonly List<IOnPostInitialize> _onPostInitializes = new();
        private readonly List<IUpdatable> _updatables = new();
        private readonly List<IFixedUpdatable> _fixedUpdatables = new();
        private readonly List<ILateUpdatable> _lateUpdatables = new();
        private readonly List<IOnPreShutdown> _onPreShutdowns = new();
        private readonly List<IOnPostShutdown> _onPostShutdowns = new();

        #region Registration Logic

        public void Register(object subscriber)
        {
            if (subscriber == null) return;

            if (subscriber is IOnPreInitialize preInit) RegisterAndSort(_onPreInitializes, preInit);
            if (subscriber is IOnPostInitialize postInit) RegisterAndSort(_onPostInitializes, postInit);
            if (subscriber is IUpdatable update) RegisterAndSort(_updatables, update);
            if (subscriber is IFixedUpdatable fixedUpdate) RegisterAndSort(_fixedUpdatables, fixedUpdate);
            if (subscriber is ILateUpdatable lateUpdate) RegisterAndSort(_lateUpdatables, lateUpdate);
            if (subscriber is IOnPreShutdown preShut) RegisterAndSort(_onPreShutdowns, preShut);
            if (subscriber is IOnPostShutdown postShut) RegisterAndSort(_onPostShutdowns, postShut);
        }

        private void RegisterAndSort<T>(List<T> list, T item)
        {
            if (list.Contains(item)) return;
            list.Add(item);
            
            // Sắp xếp dựa trên Priority mỗi khi có thành viên mới
            list.Sort((a, b) =>
            {
                int p1 = (a is IPriority prioA) ? prioA.Priority : 0;
                int p2 = (b is IPriority prioB) ? prioB.Priority : 0;
                return p1.CompareTo(p2);
            });
        }

        public void Unregister(object subscriber)
        {
            if (subscriber == null) return;

            if (subscriber is IOnPreInitialize preInit) _onPreInitializes.Remove(preInit);
            if (subscriber is IOnPostInitialize postInit) _onPostInitializes.Remove(postInit);
            if (subscriber is IUpdatable update) _updatables.Remove(update);
            if (subscriber is IFixedUpdatable fixedUpdate) _fixedUpdatables.Remove(fixedUpdate);
            if (subscriber is ILateUpdatable lateUpdate) _lateUpdatables.Remove(lateUpdate);
            if (subscriber is IOnPreShutdown preShut) _onPreShutdowns.Remove(preShut);
            if (subscriber is IOnPostShutdown postShut) _onPostShutdowns.Remove(postShut);
        }

        #endregion

        #region Internal Publish Methods

        internal void PublishPreInitialize()
        {
            _currentState = ApplicationState.Initializing;
            ExecuteList(_onPreInitializes, m => m.OnPreInitialize());
        }

        internal void PublishPostInitialize()
        {
            _currentState = ApplicationState.Running;
            ExecuteList(_onPostInitializes, m => m.OnPostInitialize());
        }

        internal void PublishUpdate(float deltaTime)
        {
            if (_currentState != ApplicationState.Running) return;
            ExecuteList(_updatables, m => m.OnUpdate(deltaTime));
        }

        internal void PublishFixedUpdate(float fixedDeltaTime)
        {
            if (_currentState != ApplicationState.Running) return;
            ExecuteList(_fixedUpdatables, m => m.OnFixedUpdatable(fixedDeltaTime));
        }

        internal void PublishLateUpdate(float deltaTime)
        {
            if (_currentState != ApplicationState.Running) return;
            ExecuteList(_lateUpdatables, m => m.OnLateUpdatable(deltaTime));
        }

        internal void PublishPreShutdown()
        {
            _currentState = ApplicationState.ShuttingDown;
            // Shutdown thường nên chạy ngược lại Priority (Service quan trọng tắt cuối cùng)
            ExecuteListReversed(_onPreShutdowns, m => m.OnPreShutdown());

            _updatables.Clear();
            _fixedUpdatables.Clear();
            _lateUpdatables.Clear();
        }

        internal void PublishPostShutdown()
        {
            _currentState = ApplicationState.Shutdown;
            ExecuteListReversed(_onPostShutdowns, m => m.OnPostShutdown());
            ClearAllSubscribers();
        }

        #endregion

        #region Execution Logic (Snapshot & Safety)

        /// <summary>
        /// Duyệt xuôi (theo Priority) và dùng Snapshot để an toàn tuyệt đối.
        /// </summary>
        private void ExecuteList<T>(List<T> list, Action<T> action)
        {
            int count = list.Count;
            if (count == 0) return;

            // Cách 1: Tạo snapshot nhanh bằng cách sao chép list
            // Điều này đảm bảo nếu trong lúc chạy action() có ai đó Register/Unregister, 
            // vòng lặp hiện tại không bị ảnh hưởng.
            T[] snapshot = list.ToArray();

            for (int i = 0; i < snapshot.Length; i++)
            {
                var item = snapshot[i];
                if (item == null) continue;

                try
                {
                    action(item);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[Lifecycle] Error in {item.GetType().Name}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Duyệt ngược cho giai đoạn Shutdown (Service quan trọng tắt sau cùng)
        /// </summary>
        private void ExecuteListReversed<T>(List<T> list, Action<T> action)
        {
            int count = list.Count;
            if (count == 0) return;

            T[] snapshot = list.ToArray();
            for (int i = snapshot.Length - 1; i >= 0; i--)
            {
                var item = snapshot[i];
                if (item == null) continue;

                try { action(item); }
                catch (Exception ex) { Debug.LogError($"[Lifecycle] Shutdown Error: {ex.Message}"); }
            }
        }

        private void ClearAllSubscribers()
        {
            _onPreInitializes.Clear();
            _onPostInitializes.Clear();
            _onPreShutdowns.Clear();
            _onPostShutdowns.Clear();
            _updatables.Clear();
            _fixedUpdatables.Clear();
            _lateUpdatables.Clear();
        }

        #endregion
    }

}
