using System;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using RestlessLib.Architecture;

namespace RestlessEngine.Diagnostics
{
    [AddComponentMenu("Restless Engine/Systems/Logger System")]
    [DisallowMultipleComponent]
    /// <summary>
    /// LogManager for Restless Engine
    /// This system is responsible for logging messages to the console with different log types.
    public class LogManager : MonoSingleton<LogManager>
    {
        public LogTag SetLogTypes = LogTag.Debug | LogTag.LifeCycle | LogTag.ApplicationSystem | LogTag.GameSystem;
        public static LogTag ActiveLogType;

        [RuntimeInitializeOnLoadMethod]
        private static void InitializeLogType()
        {
            ActiveLogType = LogTag.Debug | LogTag.LifeCycle | LogTag.ApplicationSystem | LogTag.GameSystem;
            ActiveLogType = Instance != null ? Instance.SetLogTypes : ActiveLogType;
        }

        public bool LogTypeCheckForNormalLogsOnly = true;

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Log(string message, LogTag logType = LogTag.Debug)
        {
            if ((ActiveLogType & logType) == logType)
            {
                Debug.Log($"[{logType}] - {message}");
            }
        }
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogWarning(string message, LogTag logType = LogTag.Debug)
        {
            if (Instance.LogTypeCheckForNormalLogsOnly)
            {
                Debug.LogWarning($"[{logType}] - {message}");
                return;
            }
            if ((ActiveLogType & logType) == logType)
            {
                Debug.LogWarning($"[{logType}] - {message}");
            }
        }
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogError(string message, LogTag logType = LogTag.Debug)
        {
            if (Instance.LogTypeCheckForNormalLogsOnly)
            {
                Debug.LogError($"[{logType}] - {message}");
                return;
            }
            if ((ActiveLogType & logType) == logType)
            {
                Debug.LogError($"[{logType}] - {message}");
            }
        }
    }

    [Flags]
    public enum LogTag
    {
        Debug = 1,
        LifeCycle = 2,
        ApplicationSystem = 4,
        GameSystem = 8,
        Validation = 16,
        UI = 32,
        Network = 64,
    }
}
