#if UNITY_EDITOR

#region

using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.EditorCoroutines.Editor;
using Unity.Profiling;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine.Device;
using UnityEngine.SceneManagement;

#endregion

namespace Appalachia.Utility.Editor.WakaTime
{
    internal static class WakaTime
    {
        private const string _PRF_PFX = nameof(WakaTime) + ".";
        private static readonly object _sync = new();

        private static Heartbeat _lastHeartbeat;
        private static readonly ProfilerMarker _PRF_Initialize = new(_PRF_PFX + nameof(Initialize));

        private static readonly ProfilerMarker _PRF_SendHeartbeat = new(_PRF_PFX + nameof(SendHeartbeat));

        private static readonly ProfilerMarker _PRF_SendHeartbeatInternal =
            new(_PRF_PFX + nameof(SendHeartbeatInternal));

        private static readonly ProfilerMarker _PRF_OnScriptReload = new(_PRF_PFX + nameof(OnScriptReload));

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

        internal static void SendHeartbeat(
            bool fromSave = false,
            [CallerMemberName] string callerMemberName = "")
        {
            using (_PRF_SendHeartbeat.Auto())
            {
                EditorCoroutineUtility.StartCoroutine(
                    SendHeartbeatInternal(fromSave, callerMemberName),
                    _sync
                );
            }
        }

        private static IEnumerator SendHeartbeatInternal(bool fromSave, string callerMemberName)
        {
            using (_PRF_SendHeartbeatInternal.Auto())
            {
                Logger.DebugLog(
                    $"[{callerMemberName}] Heartbeat generated - checking if it should be sent..."
                );

                var scene = SceneManager.GetActiveScene();

                yield return null;

                var scenePath = scene.path;
                var sceneFilePath = scenePath != string.Empty
                    ? Application.dataPath + "/" + scenePath.Substring("Assets/".Length)
                    : string.Empty;

                var heartbeat = new Heartbeat(sceneFilePath, fromSave);
                var timeSinceLastHeartbeat = heartbeat.time - _lastHeartbeat.time;

                var processHeartbeat = fromSave ||
                                       (timeSinceLastHeartbeat > Constants.WakaTime.HeartbeatCooldown) ||
                                       (heartbeat.entity != _lastHeartbeat.entity);

                yield return null;

                if (!processHeartbeat)
                {
                    Logger.DebugLog($"[{callerMemberName}] Skipping this heartbeat.");
                    yield break;
                }

                yield return null;

                var wakatimePath = Configuration.WakaTimePath;
                var cliTargetPath = $"\"{wakatimePath}\"";

                yield return null;
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

                yield return null;

                process.Start();

                yield return null;

                var error = process.StandardError.ReadToEnd();

                yield return null;

                var output = process.StandardOutput.ReadToEnd();

                yield return null;

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

                yield return null;
            }
        }

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
