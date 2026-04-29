using UnityEngine;

public readonly struct PointerState
{
    public readonly bool Down;
    public readonly bool Held;
    public readonly bool Up;
    public readonly Vector2 Position;

    public PointerState(bool down, bool held, bool up, Vector2 position)
    {
        Down = down;
        Held = held;
        Up = up;
        Position = position;
    }
}
