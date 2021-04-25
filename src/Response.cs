#if UNITY_EDITOR
using System;

namespace Appalachia.WakaTime
{
    [Serializable]
    internal struct Response<T>
    {
        public string error;
        public T data;
    }
}
#endif