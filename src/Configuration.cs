#if UNITY_EDITOR

using UnityEngine;

namespace Appalachia.WakaTime
{
    internal class Configuration
    {
        internal const string LogPrefix = "[WakaTime]";
        internal const string PrefPrefix = "WakaTime/";
        internal const string ApiKeyPref = PrefPrefix + "APIKey";
        internal const string EnabledPref = PrefPrefix + "Enabled";
        internal const string DebugPref = PrefPrefix + "Debug";
        internal const string WakatimeProjectFile = ".wakatime-project";
        internal const string UrlPrefix = "https://api.wakatime.com/api/v1/";
        internal const int HeartbeatCooldown = 120;
        internal static string ApiKey = "";
        internal static bool Enabled = true;
        internal static bool Debugging = true;

        internal static string GetRequestEndpoint()
        {
            return $"{Configuration.UrlPrefix}users/current/heartbeats?api_key={Configuration.ApiKey}";
        }
        
    }
}
#endif