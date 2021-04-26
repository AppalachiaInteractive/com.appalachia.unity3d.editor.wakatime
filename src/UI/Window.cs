#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Appalachia.WakaTime.UI
{
    public class Window : EditorWindow
    {
        private const string DASHBOARD_URL = "https://wakatime.com/dashboard/";
        private string _apiKey = "";
        private bool _wakatimePathAuto = true;
        private string _wakatimePath = "";
        private string _projectName = "";
        private bool _enabled = true;
        private bool _debug = true;

        private bool _needToReload;

        [MenuItem("Window/WakaTime")]
        private static void Init()
        {
            var window = (Window) GetWindow(typeof(Window), false, "WakaTime");
            window.Show();
        }

        private void OnGUI()
        {
            _needToReload = false;

            _enabled = EditorGUILayout.Toggle("Enable WakaTime", _enabled);
            
            _apiKey = EditorGUILayout.TextField("API key", _apiKey);

            EditorGUILayout.BeginHorizontal();
            _wakatimePathAuto = EditorGUILayout.ToggleLeft("WakaTime Auto Path", _wakatimePathAuto);

            GUI.enabled = !_wakatimePathAuto;
            _wakatimePath = EditorGUILayout.TextField("WakaTime Path", _wakatimePath);
            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Project name", _projectName);
            if (GUILayout.Button("Change project name"))
            {
                ProjectWindow.Display();
                _needToReload = true;
            }

            EditorGUILayout.EndHorizontal();
            
            _debug = EditorGUILayout.Toggle("Debug", _debug);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Save Preferences"))
            {
                EditorPrefs.SetBool(Configuration.WakaTimePathAutoPref, _wakatimePathAuto);
                EditorPrefs.SetString(Configuration.WakaTimePathPref, _wakatimePath);
                EditorPrefs.SetString(Configuration.ApiKeyPref,       _apiKey);
                EditorPrefs.SetBool(Configuration.EnabledPref, _enabled);
                EditorPrefs.SetBool(Configuration.DebugPref,   _debug);
                WakaTime.Initialize();
                _needToReload = _needToReload || GUI.changed;
            }

            if (GUILayout.Button("Save Preferences"))
            {
                _needToReload = true;
                CheckReloadStatus();
            }

            if (GUILayout.Button("Open Dashboard"))
            {
                Application.OpenURL(DASHBOARD_URL);
            }

            EditorGUILayout.EndHorizontal();

            CheckReloadStatus();
        }

        private void OnFocus()
        {
            CheckReloadStatus();
        }

        private void CheckReloadStatus()
        {
            if (_needToReload)
            {
                RefreshPreferences();
                WakaTime.Initialize();
                _needToReload = false;
            }
        }

        private void RefreshPreferences()
        {
            if (EditorPrefs.HasKey(Configuration.WakaTimePathAutoPref))
            {
                _wakatimePathAuto = EditorPrefs.GetBool(Configuration.WakaTimePathAutoPref);
            }

            if (EditorPrefs.HasKey(Configuration.WakaTimePathPref))
            {
                _wakatimePath = EditorPrefs.GetString(Configuration.WakaTimePathPref);
            }

            if (EditorPrefs.HasKey(Configuration.ApiKeyPref))
            {
                _apiKey = EditorPrefs.GetString(Configuration.ApiKeyPref);
            }

            if (EditorPrefs.HasKey(Configuration.EnabledPref))
            {
                _enabled = EditorPrefs.GetBool(Configuration.EnabledPref);
            }

            if (EditorPrefs.HasKey(Configuration.DebugPref))
            {
                _debug = EditorPrefs.GetBool(Configuration.DebugPref);
            }

            _projectName = WakaTime.ProjectName;
        }
    }
}

#endif
