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
    // public class ApplicationLifecycle : IApplicationLifecycle
    // {
    //     private ApplicationState _currentState;

    //     public ApplicationState CurrentState => _currentState;

    //     public event Action OnPreInitialize;
    //     public event Action OnPostInitialize;
    //     public event Action OnPreShutdown;
    //     public event Action OnPostShutdown;

    //     internal void PublishPreInitialize()
    //     {
    //         _currentState = ApplicationState.Initializing;
    //         OnPreInitialize?.Invoke();
    //     }

    //     internal void PublishPostInitialize()
    //     {
    //         _currentState = ApplicationState.Running;
    //         OnPostInitialize?.Invoke();
    //     }

    //     internal void PublishPreShutdown()
    //     {
    //         _currentState = ApplicationState.ShuttingDown;
    //         OnPreShutdown?.Invoke();
    //     }

    //     internal void PublishPostShutdown()
    //     {
    //         _currentState = ApplicationState.Shutdown;
    //         OnPostShutdown?.Invoke();
    //     }
    // }
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
            
            // Sắp xếp theo Priority (Nhỏ chạy trước)
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
            for (int i = 0; i < _onPreInitializes.Count; i++)
                try { _onPreInitializes[i].OnPreInitialize(); } catch (Exception e) { LogError(_onPreInitializes[i], e); }
        }

        internal void PublishPostInitialize()
        {
            _currentState = ApplicationState.Running;
            for (int i = 0; i < _onPostInitializes.Count; i++)
                try { _onPostInitializes[i].OnPostInitialize(); } catch (Exception e) { LogError(_onPostInitializes[i], e); }
        }

        internal void PublishUpdate(float deltaTime)
        {
            if (_currentState != ApplicationState.Running) return;
            for (int i = 0; i < _updatables.Count; i++)
                try { _updatables[i].OnUpdate(deltaTime); } catch (Exception e) { LogError(_updatables[i], e); }
        }

        internal void PublishFixedUpdate(float fixedDeltaTime)
        {
            if (_currentState != ApplicationState.Running) return;
            for (int i = 0; i < _fixedUpdatables.Count; i++)
                try { _fixedUpdatables[i].OnFixedUpdatable(fixedDeltaTime); } catch (Exception e) { LogError(_fixedUpdatables[i], e); }
        }

        internal void PublishLateUpdate(float deltaTime)
        {
            if (_currentState != ApplicationState.Running) return;
            for (int i = 0; i < _lateUpdatables.Count; i++)
                try { _lateUpdatables[i].OnLateUpdatable(deltaTime); } catch (Exception e) { LogError(_lateUpdatables[i], e); }
        }

        internal void PublishPreShutdown()
        {
            _currentState = ApplicationState.ShuttingDown;
            // Shutdown duyệt ngược (Reverse) để dọn dẹp các module phụ thuộc trước
            for (int i = _onPreShutdowns.Count - 1; i >= 0; i--)
                try { _onPreShutdowns[i].OnPreShutdown(); } catch (Exception e) { LogError(_onPreShutdowns[i], e); }

            ClearUpdateLists();
        }

        internal void PublishPostShutdown()
        {
            _currentState = ApplicationState.Shutdown;
            for (int i = _onPostShutdowns.Count - 1; i >= 0; i--)
                try { _onPostShutdowns[i].OnPostShutdown(); } catch (Exception e) { LogError(_onPostShutdowns[i], e); }

            ClearAllSubscribers();
        }

        #endregion

        #region Helpers

        private void ClearUpdateLists()
        {
            _updatables.Clear();
            _fixedUpdatables.Clear();
            _lateUpdatables.Clear();
        }

        private void ClearAllSubscribers()
        {
            _onPreInitializes.Clear();
            _onPostInitializes.Clear();
            _onPreShutdowns.Clear();
            _onPostShutdowns.Clear();
            ClearUpdateLists();
        }

        private void LogError(object item, Exception ex)
        {
            Debug.LogError($"[Lifecycle] Error in {item.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
        }

        #endregion
    }

}
