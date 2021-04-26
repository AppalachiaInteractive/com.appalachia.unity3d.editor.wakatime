#if UNITY_EDITOR

using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Appalachia.WakaTime
{
    internal static class Configuration
    {
        internal const string LogPrefix = "[WakaTime]";
        internal const string PrefPrefix = "WakaTime/";
        internal const string WakatimePathPref = PrefPrefix + "WakatimePath";
        internal const string ApiKeyPref = PrefPrefix + "APIKey";
        internal const string EnabledPref = PrefPrefix + "Enabled";
        internal const string DebugPref = PrefPrefix + "Debug";
        internal const string WakatimeProjectFile = ".wakatime-project";
        internal const int HeartbeatCooldown = 120;
        internal static string ApiKey = "";
        internal static string WakatimePath = Path.GetFullPath("Packages\\com.appalachia.unity3d.editor.wakatime\\src\\wakatime~\\wakatime\\");
        internal static bool Enabled = true;
        internal static bool Debugging = true;

        
        /*
         internal const string UrlPrefix = "https://api.wakatime.com/api/v1/";
        internal static string GetRequestEndpoint()
        {
            return $"{Configuration.UrlPrefix}users/current/heartbeats?api_key={Configuration.ApiKey}";
        }
        */
        
    }
}
#endif