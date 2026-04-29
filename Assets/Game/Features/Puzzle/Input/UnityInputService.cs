using UnityEngine;

public interface IInputPuzzleService
{
    PointerState GetPointer();
}
public sealed class UnityInputService : IInputPuzzleService
{
    public PointerState GetPointer()
    {
        // Mobile ưu tiên
        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);

            return new PointerState(
                down: touch.phase == TouchPhase.Began,
                held: touch.phase == TouchPhase.Moved 
                    || touch.phase == TouchPhase.Stationary,
                up: touch.phase == TouchPhase.Ended 
                    || touch.phase == TouchPhase.Canceled,
                position: touch.position
            );
        }

        // PC fallback (chuột)
        return new PointerState(
            down: Input.GetMouseButtonDown(0),
            held: Input.GetMouseButton(0),
            up: Input.GetMouseButtonUp(0),
            position: Input.mousePosition
        );
    }
}
