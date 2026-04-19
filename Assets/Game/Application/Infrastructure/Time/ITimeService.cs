using Game.Application.Core;

namespace Game.Application.Core.TimeService
{
    /// <summary>
    /// Example service interface.
    /// 
    /// Convention: Services are interfaces that define capabilities.
    /// Keep them small and focused (interface segregation).
    /// </summary>
    public interface ITimeService : IService
    {
        /// <summary>Current game time in seconds.</summary>
        float CurrentTime { get; }

        /// <summary>Game speed multiplier (0 = paused, 1 = normal, 2 = 2x).</summary>
        float TimeScale { set; get; }

        /// <summary>Get time info.</summary>
        GameTimeInfo GetTimeInfo();
    }

    /// <summary>
    /// Data structure for time information.
    /// Immutable to prevent accidental state corruption.
    /// </summary>
    public readonly struct GameTimeInfo
    {
        public float CurrentTime { get; }
        public float DeltaTime { get; }
        public float TimeScale { get; }
        public uint Frame { get; }

        public GameTimeInfo(float currentTime, float deltaTime, float timeScale, uint frame)
        {
            CurrentTime = currentTime;
            DeltaTime = deltaTime;
            TimeScale = timeScale;
            Frame = frame;
        }
    }
}
