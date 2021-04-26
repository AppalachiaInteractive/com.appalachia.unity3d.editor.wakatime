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
            _enabled = EditorGUILayout.Toggle("Enable WakaTime", _enabled);
            _wakatimePathAuto = EditorGUILayout.ToggleLeft("WakaTime Auto Path", _wakatimePathAuto);
            _wakatimePath = EditorGUILayout.TextField("WakaTime Path", _wakatimePath);
            _apiKey = EditorGUILayout.TextField("API key",             _apiKey);
            EditorGUILayout.LabelField("Project name", _projectName);

            if (GUILayout.Button("Change project name"))
            {
                ProjectWindow.Display();
                _needToReload = true;
            }

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
            }

            if (GUILayout.Button("Open Dashboard"))
            {
                Application.OpenURL(DASHBOARD_URL);
            }

            EditorGUILayout.EndHorizontal();
        }

        private void OnFocus()
        {
            if (_needToReload)
            {
                WakaTime.Initialize();
                _needToReload = false;
            }

            if (EditorPrefs.HasKey(Configuration.WakaTimePathAutoPref))
            {
                _wakatimePathAuto = EditorPrefs.GetBool(Configuration.WakaTimePathAutoPref);
            }

            GUI.enabled = !_wakatimePathAuto;
            
            if (EditorPrefs.HasKey(Configuration.WakaTimePathPref))
            {
                _wakatimePath = EditorPrefs.GetString(Configuration.WakaTimePathPref);
            }

            GUI.enabled = true;

            /*if (EditorPrefs.HasKey(Configuration.ApiKeyPref))
            {
                _apiKey = EditorPrefs.GetString(Configuration.ApiKeyPref);
            }*/

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
