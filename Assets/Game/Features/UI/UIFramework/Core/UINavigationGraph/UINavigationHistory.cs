using System;
using System.Collections.Generic;
public interface IUINavigationHistory
{
    void Push(UINavigationEntry entry);

    bool TryPop(out UINavigationEntry entry);
    UINavigationEntry Pop();

    UINavigationEntry Peek();

    void Clear();

    bool CanGoBack { get; }

}
public sealed class UINavigationHistory : IUINavigationHistory
{
    private readonly Stack<UINavigationEntry> _stack = new();

    public bool CanGoBack => _stack.Count > 0;

    public void Push(UINavigationEntry entry)
    {
        if (entry == null)
            throw new ArgumentNullException(nameof(entry));

        _stack.Push(entry);
    }
    public bool TryPop(out UINavigationEntry entry) 
    { 
        if (_stack.Count == 0)
        { 
            entry = null; return false; 
        } 
        entry = _stack.Pop();
        return true;
    }
    public UINavigationEntry Pop()
    {
        if (_stack.Count == 0)
            return null;

        return _stack.Pop();
    }

    public UINavigationEntry Peek()
    {
        if (_stack.Count == 0)
            return null;

        return _stack.Peek();
    }

    public void Clear()
    {
        _stack.Clear();
    }

}