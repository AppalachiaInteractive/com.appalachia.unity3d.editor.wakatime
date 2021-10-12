#if UNITY_EDITOR

#region

using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Profiling;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine.SceneManagement;
using Application = UnityEngine.Device.Application;

#endregion

namespace Appalachia.Editor.WakaTime
{
    internal static class WakaTime
    {
        private const string _PRF_PFX = nameof(WakaTime) + ".";
        
        private static Heartbeat _lastHeartbeat;
        private static readonly object _sync = new();


        private static readonly ProfilerMarker _PRF_Initialize = new ProfilerMarker(_PRF_PFX + nameof(Initialize));
        [InitializeOnLoadMethod]
        public static void Initialize()
        {
            using (_PRF_Initialize.Auto())
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


                EditorApplication.delayCall += () =>
                {
                    Logger.DebugLog("Initialized.  Sending first heartbeat...");
                    SendHeartbeat();
                    Events.LinkCallbacks();
                };
            }
        }

        private static readonly ProfilerMarker _PRF_SendHeartbeat = new ProfilerMarker(_PRF_PFX + nameof(SendHeartbeat));
        internal static void SendHeartbeat(
            bool fromSave = false,
            [CallerMemberName] string callerMemberName = "")
        {
            using (_PRF_SendHeartbeat.Auto())
            {
                Logger.DebugLog(
                    $"[{callerMemberName}] Heartbeat generated - checking if it should be sent..."
                );

                lock (_sync)
                {
                    SendHeartbeatInternal(fromSave, callerMemberName);
                }
            }
        }

        private static readonly ProfilerMarker _PRF_SendHeartbeatInternal = new ProfilerMarker(_PRF_PFX + nameof(SendHeartbeatInternal));
        private static void SendHeartbeatInternal(bool fromSave, string callerMemberName)
        {
            using (_PRF_SendHeartbeatInternal.Auto())
            {
                var scene = SceneManager.GetActiveScene();
                var scenePath = scene.path;
                var sceneFilePath = scenePath != string.Empty
                    ? Application.dataPath + "/" + scenePath.Substring("Assets/".Length)
                    : string.Empty;

                var heartbeat = new Heartbeat(sceneFilePath, fromSave);
                var timeSinceLastHeartbeat = heartbeat.time - _lastHeartbeat.time;

                var processHeartbeat = fromSave ||
                                       (timeSinceLastHeartbeat >
                                        Constants.WakaTime.HeartbeatCooldown) ||
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
                    Logger.LogError(
                        $"Unable to utilize WakaTime CLI: [{error}].  Disable this plugin."
                    );
                }
            }
        }

        private static readonly ProfilerMarker _PRF_OnScriptReload = new ProfilerMarker(_PRF_PFX + nameof(OnScriptReload));
        [DidReloadScripts]
        private static void OnScriptReload()
        {
            using (_PRF_OnScriptReload.Auto())
            {
                Logger.DebugLog("Reloading scripts..");
                Initialize();
                Logger.DebugLog("Reload completed!");
            }
        }
    }
}

#endif
