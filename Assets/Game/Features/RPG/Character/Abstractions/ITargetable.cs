using UnityEngine;
public interface ITargetable
{
    bool IsAlive { get; }
    Vector3 Position { get; }
}
