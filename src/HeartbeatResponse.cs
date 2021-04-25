#if UNITY_EDITOR

using System;

namespace Appalachia.WakaTime
{
    [Serializable]
    internal struct HeartbeatResponse
    {
        public string id;
        public string entity;
        public string type;
        public float time;
    }
}
#endif