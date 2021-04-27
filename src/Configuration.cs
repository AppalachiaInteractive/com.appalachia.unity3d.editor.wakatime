#if UNITY_EDITOR

using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;


namespace Appalachia.WakaTime
{
    internal static class Configuration
    {
        internal static string ApiKey = "";
        internal static bool WakaTimePathAuto = true;
        internal static string WakaTimePath = "";
        internal static bool Enabled = true;
        internal static bool Debugging = true;
        internal static string ProjectName = "";

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
                return $"{package.resolvedPath}";
            }

            return WakaTimePath ?? "Assets\\wakatime\\";
        }

        internal static void SavePreferences()
        {
            EditorPrefs.SetString(Constants.ConfigurationKeys.ApiKey, ApiKey);
            EditorPrefs.SetBool(Constants.ConfigurationKeys.WakaTimePathAuto, WakaTimePathAuto);
            EditorPrefs.SetString(Constants.ConfigurationKeys.WakaTimePath, WakaTimePath);
            EditorPrefs.SetBool(Constants.ConfigurationKeys.Enabled, Enabled);
            EditorPrefs.SetBool(Constants.ConfigurationKeys.Debugging, Debugging);
        }
        
        internal static void RefreshPreferences()
        {
            if (EditorPrefs.HasKey(Constants.ConfigurationKeys.WakaTimePathAuto))
            {
                WakaTimePathAuto = EditorPrefs.GetBool(Constants.ConfigurationKeys.WakaTimePathAuto);
            }

            if (EditorPrefs.HasKey(Constants.ConfigurationKeys.WakaTimePath))
            {
                WakaTimePath = EditorPrefs.GetString(Constants.ConfigurationKeys.WakaTimePath);
                if (string.IsNullOrWhiteSpace(WakaTimePath))
                {
                    WakaTimePath = GetAutoWakaTimePath();
                }
            }

            if (EditorPrefs.HasKey(Constants.ConfigurationKeys.ApiKey))
            {
                ApiKey = EditorPrefs.GetString(Constants.ConfigurationKeys.ApiKey);
            }

            if (EditorPrefs.HasKey(Constants.ConfigurationKeys.Enabled))
            {
                Enabled = EditorPrefs.GetBool(Constants.ConfigurationKeys.Enabled);
            }

            if (EditorPrefs.HasKey(Constants.ConfigurationKeys.Debugging))
            {
                Debugging = EditorPrefs.GetBool(Constants.ConfigurationKeys.Debugging);
            }
            
            ProjectName = File.Exists(Constants.Project.WakaTimeProjectFile)
                ? File.ReadAllLines(Constants.Project.WakaTimeProjectFile)[0]
                : Application.productName;
        }

    }
}
#endif
