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
        [FormerlySerializedAs("asset")]
        public Patch patch;

        [MinMax(0, 600)] public MinMaxFloat delay;

        [Colorize(order = 0)] public RandomizationParams randomization = new() {chance = 1f};

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
