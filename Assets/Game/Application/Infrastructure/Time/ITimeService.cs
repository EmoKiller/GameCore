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

        /// <summary>
        /// Lấy thông tin thời gian cho Update (Frame-based)
        /// </summary>
        GameTimeInfo GetTimeInfo();

        /// <summary>
        /// Lấy thông tin thời gian cho FixedUpdate (Physics-based)
        /// </summary>
        GameTimeInfo GetFixedTimeInfo();
    }
    public interface ITimeServiceController : ITimeService
    {
        void OnUpdate(float deltaTime);
        void OnFixedUpdatable(float fixedDeltaTime);
    }

    /// <summary>
    /// Data structure for time information.
    /// Immutable to prevent accidental state corruption.
    /// </summary>
    public readonly struct GameTimeInfo
    {
        /// <summary>
        /// Tổng thời gian đã trôi qua (scaled)
        /// </summary>
        public float CurrentTime { get; } 
        /// <summary>
        /// Thời gian khung hình hiện tại (scaled)
        /// </summary>
        public float DeltaTime { get; }
        /// <summary>
        /// Thời gian khung hình gốc từ Unity
        /// </summary>
        public float RawDeltaTime { get; }
        /// <summary>
        /// Tỉ lệ thời gian hiện tại
        /// </summary>
        public float TimeScale { get; }
        /// <summary>
        /// Số thứ tự khung hình
        /// </summary>
        public uint Frame { get; }

        public GameTimeInfo(float currentTime, float deltaTime, float rawDeltaTime, float timeScale, uint frame)
        {
            CurrentTime = currentTime;
            DeltaTime = deltaTime;
            RawDeltaTime = rawDeltaTime;
            TimeScale = timeScale;
            Frame = frame;
        }
    }
}
