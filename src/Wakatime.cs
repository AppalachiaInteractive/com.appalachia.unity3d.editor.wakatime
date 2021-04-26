#if UNITY_EDITOR

#region

using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.SceneManagement;

#endregion

namespace Appalachia.WakaTime
{
    [InitializeOnLoad]
    internal static class WakaTime
    {
        private static Heartbeat _lastHeartbeat;
        private static readonly object _sync = new object();

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

            if (EditorPrefs.HasKey(Configuration.WakaTimePathAutoPref))
            {
                Configuration.WakaTimePathAuto = EditorPrefs.GetBool(Configuration.WakaTimePathAutoPref);
            }

            if (EditorPrefs.HasKey(Configuration.WakaTimePathPref))
            {
                Configuration.WakaTimePath = EditorPrefs.GetString(Configuration.WakaTimePathPref);
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

            EditorApplication.delayCall += () =>
            {
                SendHeartbeat();
                Events.LinkCallbacks();
            };
        }

        /// <summary>
        ///     Reads .wakatime-project file
        ///     <seealso cref="https://wakatime.com/faq#rename-projects" />
        /// </summary>
        /// <returns>Lines of .wakatime-project or null if file not found</returns>
        public static string[] GetProjectFile()
        {
            return !File.Exists(Configuration.WakaTimeProjectFile) ? null : File.ReadAllLines(Configuration.WakaTimeProjectFile);
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
            File.WriteAllLines(Configuration.WakaTimeProjectFile, content);
        }

        internal static void SendHeartbeat(bool fromSave = false, [CallerMemberName] string callerMemberName = "")
        {
            Logger.DebugLog($"[{callerMemberName}] Heartbeat generated - checking if it should be sent...");

            lock (_sync)
            {
                SendHeartbeatInternal(fromSave, callerMemberName);
            }
        }

        private static void SendHeartbeatInternal(bool fromSave, string callerMemberName)
        {
            var scene = SceneManager.GetActiveScene();
            var scenePath = scene.path;
            var sceneFilePath = scenePath != string.Empty ? Application.dataPath + "/" + scenePath.Substring("Assets/".Length) : string.Empty;

            var heartbeat = new Heartbeat(sceneFilePath, fromSave);
            var timeSinceLastHeartbeat = heartbeat.time - _lastHeartbeat.time;

            var processHeartbeat =
                fromSave || (timeSinceLastHeartbeat > Configuration.HeartbeatCooldown) || (heartbeat.entity != _lastHeartbeat.entity);

            if (!processHeartbeat)
            {
                Logger.DebugLog($"[{callerMemberName}] Skipping this heartbeat.");
                return;
            }

            var basePath = Configuration.WakaTimePathAuto ? Configuration.GetAutoWakaTimePath() : Configuration.WakaTimePath;
            var wakatimePath = Path.Combine(basePath, "src\\wakatime~\\wakatime\\cli.py");
            var cliTargetPath = $"\"{wakatimePath}\"";

            var process = new Process();
            var processStartInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                FileName = "python",
                Arguments = $" {cliTargetPath} " +
                            $" --entity \"{heartbeat.entity}\"" +
                            (heartbeat.isWrite ? " --write" : string.Empty) +
                            (heartbeat.isDebugging ? " --verbose" : string.Empty) +
                            $" --entity-type \"{heartbeat.type}\"" +
                            $" --plugin \"{heartbeat.plugin}\"" +
                            $" --time \"{heartbeat.time}\"" +
                            $" --project \"{heartbeat.project}\"",
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };

            process.StartInfo = processStartInfo;

            process.Start();

            var error = process.StandardError.ReadToEnd();
            var output = process.StandardOutput.ReadToEnd();

            if (string.Empty == error)
            {
                Logger.DebugLog(output);
                Logger.DebugLog("Sent heartbeat!");
                _lastHeartbeat = heartbeat;
            }
            else
            {
                Logger.Log(processStartInfo.Arguments);
                Logger.LogError($"Unable to utilize WakaTime CLI: [{error}].  Disable this plugin.");
            }
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
            return File.Exists(Configuration.WakaTimeProjectFile) ? File.ReadAllLines(Configuration.WakaTimeProjectFile)[0] : Application.productName;
        }
    }
}

#endif
