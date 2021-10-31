#region

using Appalachia.Audio.Utilities;
using Appalachia.Core.Scriptables;
using UnityEngine;

#if UNITY_EDITOR

#endif

#endregion

namespace Appalachia.Audio.Components
{
    public class OcclusionSettings : SingletonAppalachiaObject<OcclusionSettings>
    {
        public float speedOfSound = 340f;
        public LayerMask layerMask = 1;

        [MinMax(0, 22000)] public MinMaxFloat highPassRange = new() {min = 0, max = 1100};

        [MinMax(0, 22000)] public MinMaxFloat lowPassRange = new() {min = 4400, max = 22000};
    }
}
