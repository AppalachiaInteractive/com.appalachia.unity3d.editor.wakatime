namespace Appalachia.WakaTime
{
    internal static class Constants
    {
        internal static class Language
        {
            internal const string EnableWakaTime = "Enable WakaTime";
            internal const string APIKey = "API Key";
            internal const string RefreshPreferences = "Refresh";
            internal const string SavePreferences = "Save";
            internal const string WakaTimeAutoPath = "Use Embedded WakaTime CLI";
            internal const string WakaTimePath = "WakaTime CLI Path";
            internal const string Debugging = "Debug WakaTime";
            internal const string OpenDashboard = "Open Dashboard";
            internal const string ShowAPIKey = "Show";
            internal const string HideAPIKey = "Hide";
        }

        internal static class Window
        {
            internal const string MenuPath = "Window/WakaTime";
            internal const string Title = "WakaTime";
            internal const string DashboardUrl = "https://wakatime.com/dashboard/";
        }

        internal static class Logger
        {
            internal const string LogPrefix = "[WakaTime]";
        }

        internal static class ConfigurationKeys
        {
            internal const string PreferencePrefix = "WakaTime/";
            internal const string WakaTimePathAuto = PreferencePrefix + "WakaTimePathAuto";
            internal const string WakaTimePath = PreferencePrefix + "WakaTimePath";
            internal const string ApiKey = PreferencePrefix + "APIKey";
            internal const string Enabled = PreferencePrefix + "Enabled";
            internal const string Debugging = PreferencePrefix + "Debug";
        }

        internal static class Project
        {
            internal const string WakaTimeProjectFile = ".wakatime-project";
        }

        internal static class WakaTime
        {
            internal const int HeartbeatCooldown = 120;
        }
    }
}
