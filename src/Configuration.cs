#if UNITY_EDITOR

using System.Linq;
using UnityEditor;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace Appalachia.WakaTime
{
    internal static class Configuration
    {
        internal const string LogPrefix = "[WakaTime]";
        internal const string PrefPrefix = "WakaTime/";
        internal const string WakaTimePathAutoPref = PrefPrefix + "WakaTimePathAuto";
        internal const string WakaTimePathPref = PrefPrefix + "WakaTimePath";
        internal const string ApiKeyPref = PrefPrefix + "APIKey";
        internal const string EnabledPref = PrefPrefix + "Enabled";
        internal const string DebugPref = PrefPrefix + "Debug";
        internal const string WakaTimeProjectFile = ".wakatime-project";
        internal const int HeartbeatCooldown = 120;
        internal static string ApiKey = "";
        internal static bool WakaTimePathAuto = true;
        internal static string WakaTimePath = GetAutoWakaTimePath();
        internal static bool Enabled = true;
        internal static bool Debugging = true;

        private static PackageInfo _package;

        internal static PackageInfo GetWakaTimePackage()
        {
            if (_package == null)
            {
                _package = AssetDatabase.FindAssets("package")
                                       ?.Select(AssetDatabase.GUIDToAssetPath)
                                        .Where(x => AssetDatabase.LoadAssetAtPath<TextAsset>(x) != null)
                                        .Select(PackageInfo.FindForAssetPath)
                                        .FirstOrDefault(p => (p != null) && (p.name == "com.appalachia.unity3d.editor.wakatime"));
            }

            return _package;
        }

        internal static string GetAutoWakaTimePath()
        {
            var package = GetWakaTimePackage();
            if (package != null)
            {
                return $"{package.resolvedPath}@{package.version}";
            }

            return WakaTimePath ?? "Assets\\wakatime\\";
        }

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
