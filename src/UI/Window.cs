#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Appalachia.WakaTime.UI
{
    public class Window : EditorWindow
    {
        private string _apiKey = "";
        private string _projectName = "";
        private bool _enabled = true;
        private bool _debug = true;

        private bool _needToReload;

        const string DASHBOARD_URL = "https://wakatime.com/dashboard/";

        [MenuItem("Window/WakaTime")]
        static void Init()
        {
            Window window = (Window) GetWindow(typeof(Window), false, "WakaTime");
            window.Show();
        }

        void OnGUI()
        {
            _enabled = EditorGUILayout.Toggle("Enable WakaTime", _enabled);
            _apiKey = EditorGUILayout.TextField("API key", _apiKey);
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
                EditorPrefs.SetString(Configuration.ApiKeyPref, _apiKey);
                EditorPrefs.SetBool(Configuration.EnabledPref, _enabled);
                EditorPrefs.SetBool(Configuration.DebugPref,   _debug);
                WakaTime.Initialize();
            }

            if (GUILayout.Button("Open Dashboard")) Application.OpenURL(DASHBOARD_URL);

            EditorGUILayout.EndHorizontal();
        }

        void OnFocus()
        {
            if (_needToReload)
            {
                WakaTime.Initialize();
                _needToReload = false;
            }

            if (EditorPrefs.HasKey(Configuration.ApiKeyPref)) _apiKey = EditorPrefs.GetString(Configuration.ApiKeyPref);
            if (EditorPrefs.HasKey(Configuration.EnabledPref)) _enabled = EditorPrefs.GetBool(Configuration.EnabledPref);
            if (EditorPrefs.HasKey(Configuration.DebugPref)) _debug = EditorPrefs.GetBool(Configuration.DebugPref);

            _projectName = WakaTime.ProjectName;
        }
    }
}

#endif
