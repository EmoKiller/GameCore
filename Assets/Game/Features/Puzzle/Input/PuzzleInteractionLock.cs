using UnityEngine;
public interface IPuzzleInteractionLock
{
    bool IsLocked { get; }

    void Lock();
    void Unlock();
}
public sealed class PuzzleInteractionLock : IPuzzleInteractionLock
{
    public bool IsLocked { get; private set; }

    public void Lock()
    {
        IsLocked = true;
    }

    public void Unlock()
    {
        IsLocked = false;
    }
}
