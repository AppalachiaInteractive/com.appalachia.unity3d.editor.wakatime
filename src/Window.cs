#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Appalachia.Utility.Editor.WakaTime
{
    public class Window : EditorWindow
    {
        private bool _needToReload;
        private bool _showApiKey;

        private void OnGUI()
        {
            _needToReload = false;

            Configuration.Enabled = EditorGUILayout.Toggle(
                Constants.Language.EnableWakaTime,
                Configuration.Enabled
            );

            EditorGUILayout.BeginHorizontal();
            if (_showApiKey)
            {
                Configuration.ApiKey = EditorGUILayout.TextField(
                    Constants.Language.APIKey,
                    Configuration.ApiKey
                );
            }
            else
            {
                Configuration.ApiKey = EditorGUILayout.PasswordField(
                    Constants.Language.APIKey,
                    Configuration.ApiKey
                );
            }

            EditorGUILayout.Space(3f, false);
            if (GUILayout.Button(
                _showApiKey ? Constants.Language.HideAPIKey : Constants.Language.ShowAPIKey
            ))
            {
                _showApiKey = !_showApiKey;
            }

            EditorGUILayout.EndHorizontal();

            Configuration.WakaTimePathAuto = EditorGUILayout.ToggleLeft(
                Constants.Language.WakaTimeAutoPath,
                Configuration.WakaTimePathAuto
            );

            GUI.enabled = !Configuration.WakaTimePathAuto;
            Configuration.WakaTimePath = EditorGUILayout.TextField(
                Constants.Language.WakaTimePath,
                Configuration.WakaTimePath
            );
            GUI.enabled = true;

            Configuration.Debugging = EditorGUILayout.ToggleLeft(
                Constants.Language.Debugging,
                Configuration.Debugging
            );

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(Constants.Language.SavePreferences))
            {
                Configuration.SavePreferences();
                WakaTime.Initialize();
                _needToReload = _needToReload || GUI.changed;
            }

            if (GUILayout.Button(Constants.Language.RefreshPreferences))
            {
                _needToReload = true;
                CheckReloadStatus();
            }

            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button(Constants.Language.OpenDashboard))
            {
                Application.OpenURL(Constants.Window.DashboardUrl);
            }

            CheckReloadStatus();
        }

        private void OnFocus()
        {
            CheckReloadStatus();
        }

        [MenuItem(Constants.Window.MenuPath)]
        private static void Init()
        {
            Configuration.RefreshPreferences();
            var window = (Window) GetWindow(typeof(Window), false, Constants.Window.Title);
            window.Show();
        }

        private void CheckReloadStatus()
        {
            if (_needToReload)
            {
                Configuration.RefreshPreferences();
                WakaTime.Initialize();
                _needToReload = false;
            }
        }
    }
}

#endif
