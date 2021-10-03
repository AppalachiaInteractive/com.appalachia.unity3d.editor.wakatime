#if UNITY_EDITOR

#region

using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.SceneManagement;

#endregion

namespace Appalachia.Editor.WakaTime
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
            Configuration.RefreshPreferences();
            Logger.DebugLog("Initializing...");

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

            Logger.DebugLog("Initialized.  Sending first heartbeat...");

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
            var sceneFilePath = scenePath != string.Empty
                ? Application.dataPath + "/" + scenePath.Substring("Assets/".Length)
                : string.Empty;

            var heartbeat = new Heartbeat(sceneFilePath, fromSave);
            var timeSinceLastHeartbeat = heartbeat.time - _lastHeartbeat.time;

            var processHeartbeat = fromSave ||
                                   (timeSinceLastHeartbeat > Constants.WakaTime.HeartbeatCooldown) ||
                                   (heartbeat.entity != _lastHeartbeat.entity);

            if (!processHeartbeat)
            {
                Logger.DebugLog($"[{callerMemberName}] Skipping this heartbeat.");
                return;
            }

            var wakatimePath = Configuration.WakaTimePath;
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
                Configuration.WakaTimePath = null;
                Logger.Log(processStartInfo.Arguments);
                Logger.LogError($"Unable to utilize WakaTime CLI: [{error}].  Disable this plugin.");
            }
        }

        [DidReloadScripts]
        private static void OnScriptReload()
        {
            Logger.DebugLog("Reloading scripts..");
            Initialize();
            Logger.DebugLog("Reload completed!");
        }
        
    }
}

#endif
