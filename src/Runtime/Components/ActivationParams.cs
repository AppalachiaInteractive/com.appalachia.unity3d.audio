#region

using UnityEngine;

#endregion

namespace Internal.Core.Audio
{
    public struct ActivationParams
    {
        public Transform transform;
        public Vector3 position;
        public float delay;
        public float volume;
        public float modVolume;
        public uint handle;
    }
}
