using System;
using System.Collections.Generic;
using UnityEngine;
public interface IUIRuntimeValidator
{
    void ValidatePush(UIHandle handle, Type viewType);
    void ValidatePop(UIHandle handle);

    void ValidateState(UIHandle handle, UIHandleState expected);

    void ValidateNotInMultipleContainers(
        UIHandle handle,
        IEnumerable<UIHandle> screenStack,
        IEnumerable<UIHandle> modalStack,
        IDictionary<Type, UIHandle> overlays);

    void ValidateCacheStore(UIHandle handle);
    void ValidateCacheRetrieve(UIHandle handle);

    void ValidateBeforeDispose(UIHandle handle);

}
public sealed class UIRuntimeValidator : IUIRuntimeValidator
{
    private readonly bool _throwOnError;

    public UIRuntimeValidator(bool throwOnError = true)
    {
        _throwOnError = throwOnError;
    }

    private void Fail(string message)
    {
        if (_throwOnError)
            throw new InvalidOperationException(message);

        Debug.LogError("[UI VALIDATION] " + message);
    }

    // ==============================
    // PUSH
    // ==============================

    public void ValidatePush(UIHandle handle, Type viewType)
    {
        if (handle == null)
            Fail($"Push failed: handle is null ({viewType.Name})");

        if (handle.Instance?.View == null)
            Fail($"Push failed: View is null ({viewType.Name})");

        if (handle.State == UIHandleState.Released)
            Fail($"Push failed: handle already released ({viewType.Name})");
    }

    // ==============================
    // POP
    // ==============================

    public void ValidatePop(UIHandle handle)
    {
        if (handle == null)
            Fail("Pop failed: handle is null");

        if (handle.State == UIHandleState.Released)
            Fail("Pop failed: already released");
    }

    // ==============================
    // STATE
    // ==============================

    public void ValidateState(UIHandle handle, UIHandleState expected)
    {
        if (handle.State != expected)
        {
            Fail($"State mismatch: expected {expected}, got {handle.State} ({handle.Instance.View.GetType().Name})");
        }
    }

    // ==============================
    // MULTI-CONTAINER
    // ==============================

    public void ValidateNotInMultipleContainers(
        UIHandle handle,
        IEnumerable<UIHandle> screenStack,
        IEnumerable<UIHandle> modalStack,
        IDictionary<Type, UIHandle> overlays)
    {
        int count = 0;

        foreach (var h in screenStack)
            if (h == handle) count++;

        foreach (var h in modalStack)
            if (h == handle) count++;

        foreach (var kv in overlays)
            if (kv.Value == handle) count++;

        if (count > 1)
        {
            Fail($"Handle exists in multiple containers: {handle.Instance.View.GetType().Name}");
        }
    }

    // ==============================
    // CACHE
    // ==============================

    public void ValidateCacheStore(UIHandle handle)
    {
        if (handle.State == UIHandleState.Released)
        {
            Fail($"Trying to cache released handle: {handle.Instance.View.GetType().Name}");
        }
    }

    public void ValidateCacheRetrieve(UIHandle handle)
    {
        if (handle.State == UIHandleState.Released)
        {
            Fail($"Cache returned released handle: {handle.Instance.View.GetType().Name}");
        }
    }

    // ==============================
    // DISPOSE
    // ==============================

    public void ValidateBeforeDispose(UIHandle handle)
    {
        if (handle.State == UIHandleState.Released)
        {
            Fail($"Double dispose detected: {handle.Instance.View.GetType().Name}");
        }
    }

}
