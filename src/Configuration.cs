#if UNITY_EDITOR

using System;
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
        internal static bool Enabled = true;
        internal static bool Debugging = true;
        internal static string ProjectName = "";

        [NonSerialized] private static string _wakaTimePath = "";

        [NonSerialized] private static PackageInfo _package;

        internal static string WakaTimePath
        {
            get
            {
                if (WakaTimePathAuto)
                {
                    if (_package == null)
                    {
                        _package = AssetDatabase.FindAssets("package")
                                               ?.Select(AssetDatabase.GUIDToAssetPath)
                                                .Where(x => AssetDatabase.LoadAssetAtPath<TextAsset>(x) != null)
                                                .Select(PackageInfo.FindForAssetPath)
                                                .FirstOrDefault(
                                                     p => (p != null) &&
                                                          (p.name == "com.appalachia.unity3d.editor.wakatime")
                                                 );

                    }

                    if (_package != null)
                    {
                        return $"{_package.resolvedPath}";
                    }


                    var files = Directory.EnumerateFileSystemEntries(
                        Application.dataPath,
                        "cli.py",
                        SearchOption.AllDirectories
                    );

                    foreach (var file in files)
                    {
                        _wakaTimePath = Path.GetDirectoryName(file);
                        return _wakaTimePath;
                    }
                    
                    return "Assets\\wakatime\\";
                }

                return _wakaTimePath;
            }
            set
            {
                _wakaTimePath = value;
                EditorPrefs.SetString(Constants.ConfigurationKeys.WakaTimePath, _wakaTimePath);
            }
        }

        internal static void SavePreferences()
        {
            EditorPrefs.SetString(Constants.ConfigurationKeys.ApiKey, ApiKey);
            EditorPrefs.SetBool(Constants.ConfigurationKeys.WakaTimePathAuto, WakaTimePathAuto);
            EditorPrefs.SetString(Constants.ConfigurationKeys.WakaTimePath, WakaTimePath);
            EditorPrefs.SetBool(Constants.ConfigurationKeys.Enabled,   Enabled);
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
