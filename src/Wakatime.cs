#if UNITY_EDITOR

#region

using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

#endregion

namespace Appalachia.WakaTime
{
    [InitializeOnLoad]
    internal class WakaTime
    {
        private static HeartbeatResponse _lastHeartbeat;

        static WakaTime()
        {
            Initialize();
        }

        public static string ProjectName { get; private set; }

        public static void Initialize()
        {
            if (EditorPrefs.HasKey(Configuration.EnabledPref))
            {
                Configuration.Enabled = EditorPrefs.GetBool(Configuration.EnabledPref);
            }

            if (EditorPrefs.HasKey(Configuration.DebugPref))
            {
                Configuration.Debugging = EditorPrefs.GetBool(Configuration.DebugPref);
            }

            if (!Configuration.Enabled)
            {
                Logger.DebugLog("Explicitly disabled, skipping initialization...");
                return;
            }

            if (EditorPrefs.HasKey(Configuration.ApiKeyPref))
            {
                Configuration.ApiKey = EditorPrefs.GetString(Configuration.ApiKeyPref);
            }

            if (Configuration.ApiKey == string.Empty)
            {
                Logger.LogWarning("API key is not set, skipping initialization...");
                return;
            }

            ProjectName = GetProjectName();

            Logger.DebugLog("Initializing...");

            SendHeartbeat();
            Events.LinkCallbacks();
        }

        /// <summary>
        ///     Reads .wakatime-project file
        ///     <seealso cref="https://wakatime.com/faq#rename-projects" />
        /// </summary>
        /// <returns>Lines of .wakatime-project or null if file not found</returns>
        public static string[] GetProjectFile()
        {
            return !File.Exists(Configuration.WakatimeProjectFile) ? null : File.ReadAllLines(Configuration.WakatimeProjectFile);
        }

        /// <summary>
        ///     Rewrites o creates new .wakatime-project file with given lines
        ///     <seealso cref="https://wakatime.com/faq#rename-projects" />
        /// </summary>
        /// <example>
        ///     <code>
        /// project-override-name
        /// branch-override-name
        /// </code>
        /// </example>
        /// <param name="content"></param>
        public static void SetProjectFile(string[] content)
        {
            File.WriteAllLines(Configuration.WakatimeProjectFile, content);
        }

        internal static void SendHeartbeat(bool fromSave = false)
        {
            Logger.DebugLog("Sending heartbeat...");

            var scene = SceneManager.GetActiveScene();
            var scenePath = scene.path;
            var sceneFilePath = scenePath != string.Empty ? Application.dataPath + "/" + scenePath.Substring("Assets/".Length) : string.Empty;

            var heartbeat = new Heartbeat(sceneFilePath, fromSave);
            var timeSinceLastHeartbeat = heartbeat.time - _lastHeartbeat.time;

            var processHeartbeat =
                fromSave || (timeSinceLastHeartbeat > Configuration.HeartbeatCooldown) || (heartbeat.entity != _lastHeartbeat.entity);

            if (!processHeartbeat)
            {
                Logger.DebugLog("Skipping this heartbeat.");
                return;
            }

            var heartbeatJson = JsonUtility.ToJson(heartbeat);

            var request = UnityWebRequest.Post(Configuration.GetRequestEndpoint(), string.Empty);
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(heartbeatJson));
            request.SetRequestHeader("Content-Type", "application/json");

            request.SendWebRequest().completed += operation =>
            {
                if (request.downloadHandler.text == string.Empty)
                {
                    Logger.LogWarning($"Unable to connect with WakaTime @ [{Configuration.UrlPrefix}].  Disable this plugin.");
                    return;
                }

                Logger.DebugLog($"Got response\n{request.downloadHandler.text}");

                var response = JsonUtility.FromJson<Response<HeartbeatResponse>>(request.downloadHandler.text);

                if (response.error != null)
                {
                    if (response.error == "Duplicate")
                    {
                        Logger.DebugWarn("Duplicate heartbeat");
                    }
                    else
                    {
                        Logger.LogError($"Failed to send heartbeat to WakaTime!\n{response.error}");
                    }
                }
                else
                {
                    Logger.DebugLog("Sent heartbeat!");
                    _lastHeartbeat = response.data;
                }
            };
        }

        [DidReloadScripts]
        private static void OnScriptReload()
        {
            Initialize();
        }

        /// <summary>
        ///     Project name for sending <see cref="Heartbeat" />
        /// </summary>
        /// <returns><see cref="Application.productName" /> or first line of .wakatime-project</returns>
        private static string GetProjectName()
        {
            return File.Exists(Configuration.WakatimeProjectFile) ? File.ReadAllLines(Configuration.WakatimeProjectFile)[0] : Application.productName;
        }
    }
}

#endif
