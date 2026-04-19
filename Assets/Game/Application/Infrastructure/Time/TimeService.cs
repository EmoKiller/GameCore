
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
    public class TimeService : ITimeService
    {
        private float _currentTime = 0f;
        private float _timeScale = 1f;
        private uint _frameCount = 0;
        private float _lastDeltaTime = 0f;

        public float CurrentTime => _currentTime;
        public float TimeScale 
        { 
            get => _timeScale;
            set => _timeScale = Mathf.Max(0f, value);
        }

        /// <summary>
        /// Subscribe to lifecycle for updates.
        /// Called during Initialize() by modules/bootstrap code.
        /// </summary>
        public void Initialize(IApplicationLifecycle lifecycle)
        {
            lifecycle.OnUpdate += OnLifecycleUpdate;
            Debug.Log("TimeService: Initialized and subscribed to OnUpdate.");
        }

        public GameTimeInfo GetTimeInfo()
        {
            return new GameTimeInfo(
                currentTime: _currentTime,
                deltaTime: _lastDeltaTime * _timeScale,
                timeScale: _timeScale,
                frame: _frameCount
            );
        }

        private void OnLifecycleUpdate(float deltaTime)
        {
            _lastDeltaTime = deltaTime;
            _currentTime += deltaTime * _timeScale;
            _frameCount++;
        }
    }
}
