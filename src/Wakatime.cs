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


        public static void Initialize()
        {
            Logger.Log("Initializing...");
            Configuration.RefreshPreferences();

            if (!Configuration.Enabled)
            {
                Logger.DebugLog("Explicitly disabled, skipping initialization...");
                return;
            }

            if (Configuration.ApiKey == string.Empty)
            {
                Logger.LogWarning("API key is not set, skipping initialization...");
                return;
            }

            Logger.Log("Initialized.  Sending first heartbeat...");

            EditorApplication.delayCall += () =>
            {
                SendHeartbeat();
                Events.LinkCallbacks();
            };
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
                fromSave || (timeSinceLastHeartbeat > Constants.WakaTime.HeartbeatCooldown) || (heartbeat.entity != _lastHeartbeat.entity);

            if (!processHeartbeat)
            {
                Logger.DebugLog($"[{callerMemberName}] Skipping this heartbeat.");
                return;
            }

            var basePath = Configuration.WakaTimePath;
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
                            $" --language \"{heartbeat.language}\"" +
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
            Logger.Log("Reloading scripts..");
            Initialize();
            Logger.Log("Reload completed!");
        }

    }
}

#endif
