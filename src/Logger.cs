#if UNITY_EDITOR

using UnityEngine;

namespace Appalachia.WakaTime
{
    internal class Logger
    {
        internal static string GetLogPrefix()
        {
            return $"{Configuration.LogPrefix} ";
        }

        internal static void Log(string message)
        {
            Debug.Log($"{GetLogPrefix()}{message}");
        }

        internal static void LogWarning(string message)
        {
            Debug.LogWarning($"{GetLogPrefix()}{message}");
        }

        internal static void LogError(string message)
        {
            Debug.LogError($"{GetLogPrefix()}{message}");
        }

        internal static void DebugLog(string message)
        {
            if (Configuration.Debugging)
            {
                Debug.Log($"{GetLogPrefix()}{message}");
            }
        }

        internal static void DebugWarn(string message)
        {
            if (Configuration.Debugging)
            {
                Debug.LogWarning($"{GetLogPrefix()}{message}");
            }
        }
    }
}
#endif
