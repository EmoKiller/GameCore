namespace Game.Application.Core.Logging
{
    /// <summary>
    /// Abstraction for logging to support different implementations
    /// (Unity Debug, file logging, server logging, etc.)
    /// 
    /// This decouples all framework code from UnityEngine.Debug
    /// enabling use in headless servers, CI pipelines, and tests.
    /// </summary>
    public interface ICustomLogger : IService
    {
        void Log(string message);
        void LogWarning(string message);
        void LogError(string message);
        void LogException(System.Exception exception);
    }
}
