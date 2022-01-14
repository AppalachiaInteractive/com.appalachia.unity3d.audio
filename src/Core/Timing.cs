#region

using System;
using Appalachia.Audio.Scriptables;
using Appalachia.Utility.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

#endregion

namespace Appalachia.Audio.Core
{
    [Serializable]
    public class Timing
    {
        #region Fields and Autoproperties

        [FormerlySerializedAs("asset")]
        public Patch patch;

        public RandomizationParams randomization = new() { chance = 1f };

        [MinMaxSlider(0, 600, true)]
        public Vector2 delay;

        #endregion

        public float GetDelay()
        {
            return delay.RandomValue();
        }

        #region Nested type: RandomizationParams

        [Serializable]
        public struct RandomizationParams
        {
            #region Fields and Autoproperties

            [PropertyRange(0, 1)] public float chance;

            #endregion
        }

        #endregion
    }
}
