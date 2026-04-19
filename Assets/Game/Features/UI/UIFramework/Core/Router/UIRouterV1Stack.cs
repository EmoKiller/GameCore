// using System;
// using System.Collections.Generic;
// using System.Threading;
// using Cysharp.Threading.Tasks;
// using Game.Application.UI;
// using Game.Application.UI.Core.Abstractions;
// using UnityEngine;

// public interface IUIRouter
// {
//     UniTask<UIHandle> PushAsync(Type viewType, CancellationToken ct);

//     UniTask<UIHandle> PushAsync<TView>(CancellationToken ct)
//         where TView : class, IUIView;

//     UniTask PopAsync(Type viewType, CancellationToken ct);

//     UniTask PopTopAsync(CancellationToken ct);
//     UniTask<UIHandle> ReplaceAsync(Type viewType, CancellationToken ct);
//     UIHandle Peek();
//     bool HasAny();
// }

// public sealed class UIRouter : IUIRouter
// {
//     private readonly IUIPresentationService _presentation;
//     private readonly UIManifest _manifest;

//     private readonly Stack<UIHandle> _screenStack = new();
//     private readonly Stack<UIHandle> _modalStack = new();
//     private readonly Dictionary<Type, UIHandle> _overlays = new();

//     private readonly SemaphoreSlim _lock = new(1, 1);

//     private readonly IUIRetentionCache _cache;

//     private IUIProfilerService  _profiler;
//     private readonly IUIRuntimeValidator _validator;

//     public UIRouter(
//         IUIPresentationService presentation,
//         UIManifest manifest,
//         IUIRetentionCache cache,
//         IUIProfilerService  profiler,
//         IUIRuntimeValidator validator)
//     {
//         _presentation = presentation;
//         _manifest = manifest;
//         _cache = cache;
//         _profiler = profiler;
//         _validator = validator;
//     }

//     // =====================================================
//     // PUSH
//     // =====================================================

//     public async UniTask<UIHandle> PushAsync(
//         Type viewType,
//         CancellationToken ct)
//     {
//         await _lock.WaitAsync(ct);
//         if (_screenStack.Count > 0 && _screenStack.Peek().Instance.View.GetType() == viewType)
//         {
//             return _screenStack.Peek();
//         }

//         try
//         {
//             var entry = _manifest.Get(viewType);

//             UIHandle handle;

//             // ===== CACHE HIT ===== 
//             if (_cache.TryGet(viewType, out var cached))
//             {
//                 _validator?.ValidateCacheRetrieve(cached);
//                 _cache.Remove(viewType);

//                 await cached.Instance.View.ShowAsync(ct);

//                 cached.State = UIHandleState.Active;

//                 handle = cached; // ❗ KHÔNG return
//             }
//             else
//             {
//                 handle = await _presentation.PresentAsync(viewType, ct);
//             }


//             handle.State = UIHandleState.Active;

//             _validator?.ValidatePush(handle, viewType);
//             _validator?.ValidateNotInMultipleContainers(
//                 handle,
//                 _screenStack,
//                 _modalStack,
//                 _overlays
//             );

//             switch (entry.Layer)
//             {
//                 case EUILayer.Screen:
//                     await HandleScreenAsync(handle, ct);
//                     break;

//                 case EUILayer.Popup:
//                     _modalStack.Push(handle);
//                     break;

//                 case EUILayer.Overlay:
//                     await HandleOverlayAsync(viewType, handle, ct);
//                     break;
//             }
            
//             return handle;
//         }
//         finally
//         {
//             _lock.Release();
//         }
//     }

//     public UniTask<UIHandle> PushAsync<TView>(CancellationToken ct)
//         where TView : class, IUIView
//     {
//         return PushAsync(typeof(TView), ct);
//     }

//     // =====================================================
//     // POP BY TYPE
//     // =====================================================

//     public async UniTask PopAsync(Type viewType, CancellationToken ct)
//     {
//         await _lock.WaitAsync(ct);

//         try
//         {
//             var entry = _manifest.Get(viewType);

//             switch (entry.Layer)
//             {
//                 case EUILayer.Screen:
//                     await PopScreenAsync(ct);
//                     break;

//                 case EUILayer.Popup:
//                     await PopModalAsync(viewType, ct);
//                     break;

//                 case EUILayer.Overlay:
//                     await RemoveOverlayAsync(viewType, ct);
//                     break;
                
//             };
            
//         }
//         finally
//         {
//             _lock.Release();
//         }
//     }

//     // =====================================================
//     // POP TOP (FOR NAVIGATOR BACK)
//     // =====================================================

//     public async UniTask PopTopAsync(CancellationToken ct)
//     {
//         await _lock.WaitAsync(ct);

//         try
//         {
//             if (_modalStack.Count > 0)
//             {
//                 await PopModalAsync(null, ct);
//                 return;
//             }

//             if (_screenStack.Count > 0)
//             {
//                 await PopScreenAsync(ct);
//             }
//         }
//         finally
//         {
//             _lock.Release();
//         }
//     }

//     // =====================================================
//     // INTERNAL HANDLERS
//     // =====================================================

//     private async UniTask HandleScreenAsync(UIHandle handle, CancellationToken ct)
//     {
//         // hide current nhưng KHÔNG dispose
//         if (_screenStack.Count > 0)
//         {
//             var current = _screenStack.Peek();

//             // tránh push duplicate
//             if (current.Instance.View.GetType() == handle.Instance.View.GetType())
//                 return; 

//             await current.Instance.View.HideAsync(ct);
//             current.State = UIHandleState.Hidden;

//             _profiler.Record(new UIProfilerEvent(
//                 UIProfilerEventType.Show,
//                 current.Instance.View.GetType(),
//                 Time.time));
//         }

//         _screenStack.Push(handle);
//         EnforceRetentionLimit(handle.Instance.View.GetType());
//     }

//     private async UniTask HandleOverlayAsync(
//         Type viewType,
//         UIHandle handle,
//         CancellationToken ct)
//     {
//         if (_overlays.TryGetValue(viewType, out var existing))
//         {
//             // xử lý theo policy
//             await RemoveOverlayInternalAsync(viewType, existing, ct);
//             _overlays.Remove(viewType);
//         }
//         _validator?.ValidatePush(handle, viewType);

//         _overlays[viewType] = handle;

//         handle.State = UIHandleState.Active;

//     }

//     private async UniTask PopScreenAsync(CancellationToken ct)
//     {
//         if (_screenStack.Count == 0)
//             return;

//         var top = _screenStack.Pop();

//         _validator?.ValidatePop(top);

//         var type = top.Instance.View.GetType();
//         var entry = _manifest.Get(type);

//         switch (entry.ReusePolicy)
//         {
//             case UIReusePolicy.Retain:
//             {
//                 await top.Instance.View.HideAsync(ct);
//                 top.State = UIHandleState.Hidden;
//                 break;
//             }

//             case UIReusePolicy.Cache:
//                 _validator?.ValidateCacheStore(top);

//                 await top.Instance.View.HideAsync(ct);
//                 await _cache.StoreAsync(type, top);
//                 top.State = UIHandleState.Cached;
//                 break;

//             case UIReusePolicy.Release:
//                 await _presentation.DismissAsync(top, ct);
//                 top.State = UIHandleState.Released;
//                 break;
//         }

//         _profiler.Record(new UIProfilerEvent(
//             UIProfilerEventType.Hide,
//             type,
//             Time.time));

//         // restore previous
//         if (_screenStack.Count > 0)
//         {
//             var previous = _screenStack.Peek();
//             previous.State = UIHandleState.Active;
//             await previous.Instance.View.ShowAsync(ct);
//         }

//     }

//     private async UniTask PopModalAsync(Type viewType, CancellationToken ct)
//     {
//         if (_modalStack.Count == 0)
//             return;

//         var top = _modalStack.Peek();

//         if (viewType != null && top.Instance.View.GetType() != viewType)
//             return;

//         var handle = _modalStack.Pop();

//         _validator?.ValidatePop(handle);

//         await _presentation.DismissAsync(handle, ct);
//     }

//     public async UniTask RemoveOverlayAsync(
//         Type viewType,
//         CancellationToken ct)
//     {
//         if (!_overlays.TryGetValue(viewType, out var handle))
//             return;

//         await RemoveOverlayInternalAsync(viewType, handle, ct);

//         _overlays.Remove(viewType);

//     }

//     // =====================================================
//     // QUERY
//     // =====================================================

//     public UIHandle Peek()
//     {
//         if (_modalStack.Count > 0)
//             return _modalStack.Peek();

//         if (_screenStack.Count > 0)
//             return _screenStack.Peek();

//         return null;
//     }
//     // =====================================================
//     // RETENTION DEPTH LIMIT
//     // =====================================================
//     private void EnforceRetentionLimit(Type viewType)
//     {
//         var entry = _manifest.Get(viewType);

//         if (entry.ReusePolicy != UIReusePolicy.Retain)
//             return;

//         var arr = _screenStack.ToArray(); // snapshot

//         int count = 0;

//         for (int i = arr.Length - 1; i >= 0; i--)
//         {
//             var handle = arr[i];

//             if (handle.Instance.View.GetType() != viewType)
//                 continue;

//             count++;

//             if (count > entry.RetainDepth)
//             {
//                 // downgrade (không modify stack trực tiếp)
//                 handle.Instance.View.HideAsync(default).Forget();
//                 handle.State = UIHandleState.Hidden;
//             }
//         }
//     }
//     private async UniTask RemoveOverlayInternalAsync(
//         Type viewType,
//         UIHandle handle,
//         CancellationToken ct)
//     {
//         var entry = _manifest.Get(viewType);
//         var view = handle.Instance.View;

//         switch (entry.ReusePolicy)
//         {
//             case UIReusePolicy.Retain:
//             {
//                 await view.HideAsync(ct);
//                 handle.State = UIHandleState.Hidden;
//                 break;
//             }

//             case UIReusePolicy.Cache:
//             {
//                 _validator?.ValidateCacheStore(handle);

//                 await view.HideAsync(ct);
//                 await _cache.StoreAsync(viewType, handle);
//                 handle.State = UIHandleState.Cached;
//                 break;
//             }

//             case UIReusePolicy.Release:
//             {
//                 await _presentation.DismissAsync(handle, ct);
//                 handle.State = UIHandleState.Released;
//                 break;
//             }
//         }

//     }

//     // =====================================================
//     // ReplaceAsync
//     // =====================================================
//     public async UniTask<UIHandle> ReplaceAsync( Type viewType, CancellationToken ct)
//     {
//         // remove current
//         if (_screenStack.Count > 0)
//         {
//             var current = _screenStack.Pop();
//             await PopHandleAsync(current, ct);
//         }

//         // push new
//         return await PushAsync(viewType, ct);

//     }
//     private async UniTask PopHandleAsync( UIHandle handle, CancellationToken ct)
//     {
//         var type = handle.Instance.View.GetType();
//         var entry = _manifest.Get(type);

//         switch (entry.ReusePolicy)
//         {
//             case UIReusePolicy.Retain:
//                 await handle.Instance.View.HideAsync(ct);
//                 handle.State = UIHandleState.Hidden;
//                 break;

//             case UIReusePolicy.Cache:
//                 await handle.Instance.View.HideAsync(ct);
//                 await _cache.StoreAsync(type, handle);
//                 handle.State = UIHandleState.Cached;
//                 break;

//             case UIReusePolicy.Release:
//                 await _presentation.DismissAsync(handle, ct);
//                 handle.State = UIHandleState.Released;
//                 break;
//         }

//     }

//     public bool HasAny()
//     {
//         return _screenStack.Count > 0
//             || _modalStack.Count > 0
//             || _overlays.Count > 0;
//     }
//     public IReadOnlyCollection<UIHandle> GetScreenStack()
//     {
//         return _screenStack;
//     }

// }

    
