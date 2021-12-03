#region

using System;
using Appalachia.Audio.Scriptables;
using Appalachia.Audio.Utilities;
using Appalachia.Utility.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Appalachia.Audio.Core
{
    [Serializable]
    public class AudioSequence
    {
        #region Fields and Autoproperties

        [MinMaxSlider(0, 600, true)]
        public Vector2 duration;

        [NonSerialized] public Patch patch;

        public RepeatParams repeat = new() { forever = true };
        public Timing[] timing;

        [NonSerialized] public uint lastHandle;

        #endregion

        public bool Activate(ActivationParams ap)
        {
            if ((lastHandle != ap.handle) || !Application.isPlaying)
            {
                lastHandle = ap.handle;
                return Synthesizer.Activate(this, ap);
            }

            return false;
        }

        public bool GetCueInfo(out float duration, out int repeats)
        {
            duration = GetDuration();
            repeats = repeat.forever ? -1 : repeat.count;
            return duration > Mathf.Epsilon;
        }

        public float GetDuration()
        {
            return duration.RandomValue();
        }

        internal float GetMaxDuration()
        {
            return duration.y * (1 + repeat.count);
        }

        #region Nested type: RepeatParams

        [Serializable]
        public struct RepeatParams
        {
            #region Fields and Autoproperties

            public bool forever;
            [PropertyRange(0, 99)] public int count;

            #endregion
        }

        #endregion
    }
}
