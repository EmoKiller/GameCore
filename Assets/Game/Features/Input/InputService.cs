using System;
using UnityEngine.InputSystem;
using Game.Application.Core;
using UnityEngine;
using Game.Application.Events;

namespace Game.Application.Core.Input
{
    public interface IInputService : IService
    {
        PlayerInputAdapter GetPlayerInput();
        InputDeviceType GetCurrentDevice();

    }
    public sealed class InputService : IInputService , IDisposable
    {
        private readonly PlayerInput _playerInput;
        private PlayerInputAdapter _playerInputAdapter;
        private readonly InputDeviceDetector _inputDeviceDetector;
 
        public InputService(
            PlayerInput playerInput,
            PlayerInputAdapter playerInputAdapter,
            InputDeviceDetector inputDeviceDetector
            )
        {
            _playerInput = playerInput;
            _playerInputAdapter = playerInputAdapter;
            _inputDeviceDetector = inputDeviceDetector;

            _playerInput.onControlsChanged += HandleDeviceChanged;
        }
        private void HandleDeviceChanged(PlayerInput input)
        {
            _inputDeviceDetector.Detect(input.currentControlScheme);
        }
        public PlayerInputAdapter GetPlayerInput()
        {
            return _playerInputAdapter;
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
