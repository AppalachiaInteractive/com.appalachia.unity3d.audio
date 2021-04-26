#region

using System;
using UnityEngine;
using UnityEngine.Serialization;

#endregion

namespace Internal.Core.Audio
{
    [Serializable]
    public class Timing
    {
        [FormerlySerializedAs("asset")]
        public Patch patch;

        [MinMax(0, 600)] public MinMaxFloat delay;

        [Colorize(order = 0)] public RandomizationParams randomization = new RandomizationParams {chance = 1f};

        public float GetDelay()
        {
            return delay.GetRandomValue();
        }

        [Serializable]
        public struct RandomizationParams
        {
            [Range(0, 1)] public float chance;
        }
    }
}
