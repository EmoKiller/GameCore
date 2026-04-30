using Game.Application.Core;
using UnityEngine;

public interface IInputPuzzleService : IService
{
    PointerState GetPointer();
}
public sealed class UnityInputService : IInputPuzzleService
{
    public PointerState GetPointer()
    {
        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);

            return new PointerState(
                pressed: touch.phase == TouchPhase.Began,
                holding:
                    touch.phase == TouchPhase.Moved ||
                    touch.phase == TouchPhase.Stationary,
                released:
                    touch.phase == TouchPhase.Ended ||
                    touch.phase == TouchPhase.Canceled,
                screenPosition: touch.position
            );
        }

        return new PointerState(
            pressed: Input.GetMouseButtonDown(0),
            holding: Input.GetMouseButton(0),
            released: Input.GetMouseButtonUp(0),
            screenPosition: Input.mousePosition
        );
    }
}
