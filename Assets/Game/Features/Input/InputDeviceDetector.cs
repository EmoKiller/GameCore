using System;
using Game.Application.Events;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Application.Core.Input
{
    public enum InputDeviceType
    {
        Gamepad,
        KeyboardMouse,
        Touch
    }

    public sealed class InputDeviceDetector
    {
        public InputDeviceType CurrentDevice { get; private set; }
        public event Action<InputDeviceType> OnDeviceChanged;

        public void Detect(string scheme)
        {
            var newDevice = scheme switch
            {
                "KeyboardMouse" => InputDeviceType.KeyboardMouse,
                "Gamepad" => InputDeviceType.Gamepad,
                "Touch" => InputDeviceType.Touch,
                _ => InputDeviceType.KeyboardMouse
            };

            SetDevice(newDevice);
            
            GameApplication.Instance.Services.Resolve<IEventBus>().Publish(
                new InputDeviceOnChanger(newDevice),
                EventChannel.Input
                );
        }

        private void SetDevice(InputDeviceType device)
        {
            if (CurrentDevice == device) return;

            CurrentDevice = device;
            OnDeviceChanged?.Invoke(device);
        }
    }
}
