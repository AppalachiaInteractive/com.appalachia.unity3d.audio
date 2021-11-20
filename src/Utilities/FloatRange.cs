using System;
using UnityEngine;

namespace Appalachia.Audio.Utilities
{
    [Serializable]
    public struct FloatRange
    {
        #region Fields and Autoproperties

        public float max;
        public float min;

        #endregion

        public float GetClampedValue(float v)
        {
            return Mathf.Clamp(v, min, max);
        }

        public float GetRandomValue()
        {
            return GetRangedValue(Randomizer.zeroToOne);
        }

        public float GetRangedValue(float v)
        {
            return (v * (max - min)) + min;
        }
    }
}
