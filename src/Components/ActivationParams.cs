#region

using UnityEngine;

#endregion

namespace Appalachia.Core.Audio.Components
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
