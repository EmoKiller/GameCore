
using UnityEngine;

namespace Game.Application.Core.TimeService
{
    
    /// <summary>
    /// Concrete implementation of ITimeService.
    /// 
    /// This service manages game time tracking.
    /// It tracks elapsed time and applies time scale (for pause, slow-mo, etc).
    /// 
    /// Pattern:
    /// - Implements service interface
    /// - Also implements IInitializable for async setup if needed
    /// - Receives dependencies via Initialize() from container
    /// - Listens to lifecycle events for updates
    /// - Stateless except for time tracking
    /// </summary>
    public class TimeService : ITimeServiceController 
    {
        private float _currentTime = 0f;
        private float _currentFixedTime = 0f;
        private float _timeScale = 1f;
        private uint _frameCount = 0;
        
        private float _lastRawDeltaTime = 0f;
        private float _lastFixedRawDeltaTime = 0f;

        public float CurrentTime => _currentTime;
        public float TimeScale 
        { 
            get => _timeScale;
            set => _timeScale = Mathf.Max(0f, value);
        }

        public GameTimeInfo GetTimeInfo()
        {
            return new GameTimeInfo(
                currentTime: _currentTime,
                deltaTime: _lastRawDeltaTime * _timeScale,
                rawDeltaTime: _lastRawDeltaTime,
                timeScale: _timeScale,
                frame: _frameCount
            );
        }

        public GameTimeInfo GetFixedTimeInfo()
        {
            return new GameTimeInfo(
                currentTime: _currentFixedTime, 
                deltaTime: _lastFixedRawDeltaTime * _timeScale,
                rawDeltaTime: _lastFixedRawDeltaTime,
                timeScale: _timeScale,
                frame: _frameCount
            );
        }

        // Được gọi từ ApplicationLifecycle qua IUpdatable
        public void OnUpdate(float deltaTime)
        {
            _lastRawDeltaTime = deltaTime;
            _currentTime += deltaTime * _timeScale;
            _frameCount++;
        }

        // Được gọi từ ApplicationLifecycle qua IFixedUpdatable
        public void OnFixedUpdatable(float fixedDeltaTime)
        {
            _lastFixedRawDeltaTime = fixedDeltaTime;
            _currentFixedTime += fixedDeltaTime * _timeScale;
        }
    }

}
