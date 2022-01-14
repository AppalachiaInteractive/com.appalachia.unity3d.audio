#region

using Appalachia.Core.Objects.Root;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Appalachia.Audio.Scriptables
{
    public class OcclusionSettings : SingletonAppalachiaObject<OcclusionSettings>
    {
        #region Fields and Autoproperties

        public float speedOfSound = 340f;
        public LayerMask layerMask = 1;

        [MinMaxSlider(0, 22000, true)]
        public Vector2 highPassRange = new() { x = 0, y = 1100 };

        [MinMaxSlider(0, 22000, true)]
        public Vector2 lowPassRange = new() { x = 4400, y = 22000 };

        #endregion
    }
}
