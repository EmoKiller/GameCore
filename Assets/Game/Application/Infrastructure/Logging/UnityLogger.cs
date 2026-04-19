using UnityEngine;

namespace Game.Application.Core.Logging
{
    /// <summary>
    /// Unity-specific logger implementation.
    /// Uses UnityEngine.Debug for all output.
    /// </summary>
    public class UnityLogger : ICustomLogger 
    {
        public string ServiceName => nameof(UnityLogger);

        public void Log(string message)
        {
            Debug.Log(message);
        }

        public void LogWarning(string message)
        {
            Debug.LogWarning(message);
        }

        public void LogError(string message)
        {
            Debug.LogError(message);
        }

        public void LogException(System.Exception exception)
        {
            Debug.LogException(exception);
        }
    }
}
