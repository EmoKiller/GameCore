using UnityEngine;
using UnityEngine.InputSystem;
public interface IPointerInputReader
{
    bool WasPressedThisFrame();
    bool WasReleasedThisFrame();
    bool IsPressed();
    Vector2 GetPosition();
}
public sealed class InputSystemPointerReader : IPointerInputReader
{
    private readonly InputAction _pressAction;
    private readonly InputAction _positionAction;

    private bool _pressedThisFrame;
    private bool _releasedThisFrame;

    public InputSystemPointerReader(
        PlayerInput playerInput)
    {
        _pressAction =
            playerInput.actions["Press"];

        _positionAction =
            playerInput.actions["Position"];

        _pressAction.started += OnPressStarted;
        _pressAction.canceled += OnPressCanceled;
    }

    private void OnPressStarted(InputAction.CallbackContext context)
    {
        _pressedThisFrame = true;
    }

    private void OnPressCanceled(InputAction.CallbackContext context)
    {
        _releasedThisFrame = true;
    }

    public bool WasPressedThisFrame()
    {
        bool result = _pressedThisFrame;
        _pressedThisFrame = false;

        return result;
    }

    public bool WasReleasedThisFrame()
    {
        bool result = _releasedThisFrame;
        _releasedThisFrame = false;

        return result;
    }
    public bool IsPressed()
    {
        return Pointer.current != null &&
               Pointer.current.press.isPressed;
    }
    public Vector2 GetPosition()
    {
        if (Pointer.current == null)
        {
            return Vector2.zero;
        }
        return _positionAction.ReadValue<Vector2>();
    }
}