using Game.Application.Core.Input;
using Game.Application.Events;

public readonly struct InputDeviceOnChanger : IEvent
{
    public readonly InputDeviceType CurrentDevice;
    public InputDeviceOnChanger(InputDeviceType currentDevice)
    {
        CurrentDevice = currentDevice;
    }
}
