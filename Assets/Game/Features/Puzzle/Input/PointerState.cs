using UnityEngine;


public readonly struct PointerState
{
    public readonly bool Pressed;
    public readonly bool Holding;
    public readonly bool Released;

    public readonly Vector2 ScreenPosition;

    public PointerState(
        bool pressed,
        bool holding,
        bool released,
        Vector2 screenPosition)
    {
        Pressed = pressed;
        Holding = holding;
        Released = released;
        ScreenPosition = screenPosition;
    }
}
