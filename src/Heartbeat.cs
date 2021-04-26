#if UNITY_EDITOR

using System;

namespace Appalachia.WakaTime
{
    internal struct Heartbeat
    {
        private static readonly DateTime _epoch = new DateTime(1970, 1, 1);

        public string entity;
        public string type;
        public float time;
        public string project;
        public string branch;
        public string plugin;
        public string language;
        public bool isWrite;
        public bool isDebugging;

        public Heartbeat(string file, bool save = false)
        {
            entity = file == string.Empty ? "Unsaved Scene" : file;
            type = "file";
            time = (float) DateTime.UtcNow.Subtract(_epoch).TotalSeconds;
            project = WakaTime.ProjectName;
            plugin = "unity-wakatime";
            branch = "main";
            language = "unity";
            isWrite = save;
            isDebugging = Configuration.Debugging;
        }
    }
}
#endif
