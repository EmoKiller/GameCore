using System;
using UnityEngine.InputSystem;
using Game.Application.Core;
using UnityEngine;

namespace Game.Application.Core.Input
{
    public interface IInputService : IService
    {
        InputDeviceType GetCurrentDevice();

    }
    public sealed class InputService : IInputService , IDisposable
    {
        private readonly PlayerInput _playerInput;
        private readonly InputDeviceDetector _inputDeviceDetector;
 
        public InputService(
            PlayerInput playerInput,
            InputDeviceDetector inputDeviceDetector
            )
        {
            _playerInput = playerInput;
            _inputDeviceDetector = inputDeviceDetector;

            _playerInput.onControlsChanged += HandleDeviceChanged;
        }
        private void HandleDeviceChanged(PlayerInput input)
        {
            _inputDeviceDetector.Detect(input.currentControlScheme);
        }
        public InputDeviceType GetCurrentDevice()
        {
            return _inputDeviceDetector.CurrentDevice;
        }
        public void Dispose()
        {
            if(_playerInput != null)
            {
                _playerInput.onControlsChanged -= HandleDeviceChanged;
                
            }
                
            
        }
    }
}
