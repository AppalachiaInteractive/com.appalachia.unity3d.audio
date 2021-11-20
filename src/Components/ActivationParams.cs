#region

using UnityEngine;

#endregion

namespace Appalachia.Audio.Components
{
    public struct ActivationParams
    {
        #region Fields and Autoproperties

        public float delay;
        public float modVolume;
        public float volume;
        public Transform transform;
        public uint handle;
        public Vector3 position;

        #endregion
    }
}
