#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Appalachia.WakaTime
{
    internal class Events
    {



        internal static void LinkCallbacks(bool clean = false)
        {
            if (clean)
            {
                EditorApplication.contextualPropertyMenu -= ContextualPropertyMenu;
                EditorApplication.hierarchyChanged -= EditorApplicationOnhierarchyChanged;
                EditorApplication.projectChanged -= EditorApplicationOnprojectChanged;
                EditorApplication.searchChanged -= SearchChanged;
                EditorApplication.modifierKeysChanged -= ModifierKeysChanged;
                EditorApplication.quitting -= EditorApplicationOnquitting;
                EditorApplication.pauseStateChanged -= EditorApplicationOnpauseStateChanged;
                EditorApplication.playModeStateChanged -= EditorApplicationOnplayModeStateChanged;
                EditorSceneManager.sceneLoaded -= EditorSceneManagerOnsceneLoaded;
                EditorSceneManager.sceneUnloaded -= EditorSceneManagerOnsceneUnloaded;
                EditorSceneManager.sceneOpened -= EditorSceneManagerOnsceneOpened;
                EditorSceneManager.sceneClosed -= EditorSceneManagerOnsceneClosed;
                EditorSceneManager.newSceneCreated -= EditorSceneManagerOnnewSceneCreated;
                //EditorSceneManager.sceneSaved -= EditorSceneManagerOnsceneSaved;
                EditorSceneManager.sceneDirtied -= EditorSceneManagerOnsceneDirtied;
            }

            EditorApplication.contextualPropertyMenu += ContextualPropertyMenu;
            EditorApplication.hierarchyChanged += EditorApplicationOnhierarchyChanged;
            EditorApplication.projectChanged += EditorApplicationOnprojectChanged;
            EditorApplication.searchChanged += SearchChanged;
            EditorApplication.modifierKeysChanged += ModifierKeysChanged;
            EditorApplication.quitting += EditorApplicationOnquitting;
            EditorApplication.pauseStateChanged += EditorApplicationOnpauseStateChanged;
            EditorApplication.playModeStateChanged += EditorApplicationOnplayModeStateChanged;
            EditorSceneManager.sceneLoaded += EditorSceneManagerOnsceneLoaded;
            EditorSceneManager.sceneUnloaded += EditorSceneManagerOnsceneUnloaded;
            EditorSceneManager.sceneOpened += EditorSceneManagerOnsceneOpened;
            EditorSceneManager.sceneClosed += EditorSceneManagerOnsceneClosed;
            EditorSceneManager.newSceneCreated += EditorSceneManagerOnnewSceneCreated;
            //EditorSceneManager.sceneSaved += EditorSceneManagerOnsceneSaved;
            EditorSceneManager.sceneDirtied += EditorSceneManagerOnsceneDirtied;
        }

        private static void EditorSceneManagerOnsceneDirtied(Scene scene)
        {
            WakaTime.SendHeartbeat();
        }

        private static void EditorSceneManagerOnsceneSaved(Scene scene)
        {
            WakaTime.SendHeartbeat(true);
        }

        private static void EditorSceneManagerOnnewSceneCreated(Scene scene, NewSceneSetup setup, NewSceneMode mode)
        {
            WakaTime.SendHeartbeat();
        }

        private static void EditorSceneManagerOnsceneClosed(Scene scene)
        {
            WakaTime.SendHeartbeat(true);
        }

        private static void EditorSceneManagerOnsceneOpened(Scene scene, OpenSceneMode mode)
        {
            WakaTime.SendHeartbeat();
        }

        private static void EditorSceneManagerOnsceneUnloaded(Scene arg0)
        {
            WakaTime.SendHeartbeat();
        }

        private static void EditorSceneManagerOnsceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            WakaTime.SendHeartbeat();
        }

        private static void EditorApplicationOnpauseStateChanged(PauseState obj)
        {
            WakaTime.SendHeartbeat();
        }

        private static void EditorApplicationOnquitting()
        {
            WakaTime.SendHeartbeat(true);
        }

        private static void ModifierKeysChanged()
        {
            WakaTime.SendHeartbeat();
        }

        private static void SearchChanged()
        {
            WakaTime.SendHeartbeat();
        }

        private static void EditorApplicationOnprojectChanged()
        {
            WakaTime.SendHeartbeat();
        }

        private static void EditorApplicationOnhierarchyChanged()
        {
            WakaTime.SendHeartbeat();
        }

        private static void ContextualPropertyMenu(GenericMenu menu, SerializedProperty property)
        {
            WakaTime.SendHeartbeat();
        }

        private static void EditorApplicationOnplayModeStateChanged(PlayModeStateChange obj)
        {
            WakaTime.SendHeartbeat();
        }
    }

}
#endif