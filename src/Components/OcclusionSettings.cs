#region

using Appalachia.Audio.Utilities;
using Appalachia.Core.Scriptables;
using UnityEngine;

#endregion

namespace Appalachia.Audio.Components
{
    public class OcclusionSettings : SingletonAppalachiaObject<OcclusionSettings>
    {
        #region Fields and Autoproperties

        public float speedOfSound = 340f;
        public LayerMask layerMask = 1;

        [FloatRange(0, 22000)] public FloatRange highPassRange = new() {min = 0, max = 1100};

        [FloatRange(0, 22000)] public FloatRange lowPassRange = new() {min = 4400, max = 22000};

        #endregion
    }
}
