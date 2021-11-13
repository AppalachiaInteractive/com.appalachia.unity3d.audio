#region

using System;
using Appalachia.Audio.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

#endregion

namespace Appalachia.Audio.Components
{
    [Serializable]
    public class Timing
    {
        #region Fields

        [FloatRange(0, 600)] public FloatRange delay;

        [FormerlySerializedAs("asset")]
        public Patch patch;

        [Colorize(order = 0)] public RandomizationParams randomization = new() {chance = 1f};

        #endregion

        public float GetDelay()
        {
            return delay.GetRandomValue();
        }

        #region Nested type: RandomizationParams

        [Serializable]
        public struct RandomizationParams
        {
            #region Fields

            [Range(0, 1)] public float chance;

            #endregion
        }

        #endregion
    }
}
