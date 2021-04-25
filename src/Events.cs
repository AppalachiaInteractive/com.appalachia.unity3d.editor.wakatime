#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Appalachia.WakaTime
{
    internal class Events
    {

        private static void OnPlaymodeStateChanged(PlayModeStateChange change)
        {
            WakaTime.SendHeartbeat();
        }

        private static void OnPropertyContextMenu(GenericMenu menu, SerializedProperty property)
        {
            WakaTime.SendHeartbeat();
        }

        private static void OnHierarchyWindowChanged()
        {
            WakaTime.SendHeartbeat();
        }

        private static void OnSceneSaved(Scene scene)
        {
            WakaTime.SendHeartbeat(true);
        }

        private static void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            WakaTime.SendHeartbeat();
        }

        private static void OnSceneClosing(Scene scene, bool removingScene)
        {
            WakaTime.SendHeartbeat();
        }

        private static void OnSceneCreated(Scene scene, NewSceneSetup setup, NewSceneMode mode)
        {
            WakaTime.SendHeartbeat();
        }

        private static void EditorSceneManagerOnactiveSceneChangedInEditMode(Scene arg0, Scene arg1)
        {
            WakaTime.SendHeartbeat();
        }

        private static void EditorSceneManagerOnactiveSceneChanged(Scene arg0, Scene arg1)
        {
            WakaTime.SendHeartbeat();
        }

        internal static void LinkCallbacks(bool clean = false)
        {
            if (clean)
            {
                EditorApplication.playModeStateChanged -= OnPlaymodeStateChanged;
                EditorApplication.contextualPropertyMenu -= OnPropertyContextMenu;
                EditorApplication.hierarchyChanged -= OnHierarchyWindowChanged;
                EditorSceneManager.sceneSaved -= OnSceneSaved;
                EditorSceneManager.sceneOpened -= OnSceneOpened;
                EditorSceneManager.sceneClosing -= OnSceneClosing;
                EditorSceneManager.newSceneCreated -= OnSceneCreated;
                EditorSceneManager.activeSceneChangedInEditMode -= EditorSceneManagerOnactiveSceneChangedInEditMode;
                EditorSceneManager.activeSceneChanged -= EditorSceneManagerOnactiveSceneChanged;
            }

            EditorApplication.playModeStateChanged += OnPlaymodeStateChanged;
            EditorApplication.contextualPropertyMenu += OnPropertyContextMenu;
            EditorApplication.hierarchyChanged += OnHierarchyWindowChanged;
            EditorSceneManager.sceneSaved += OnSceneSaved;
            EditorSceneManager.sceneOpened += OnSceneOpened;
            EditorSceneManager.sceneClosing += OnSceneClosing;
            EditorSceneManager.newSceneCreated += OnSceneCreated;
            EditorSceneManager.activeSceneChangedInEditMode += EditorSceneManagerOnactiveSceneChangedInEditMode;
            EditorSceneManager.activeSceneChanged += EditorSceneManagerOnactiveSceneChanged;
        }
    }
}
#endif