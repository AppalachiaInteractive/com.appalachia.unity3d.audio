#region

using System;
using Appalachia.Audio.Utilities;
using UnityEngine;

#endregion

namespace Appalachia.Audio.Components
{
    [Serializable]
    public class AudioSequence
    {
        [Space(10)]
        [MinMax(0, 600, colorize = true)]
        public MinMaxFloat duration;

        [NonSerialized] public Patch patch;

        [Colorize] public RepeatParams repeat = new() {forever = true};
        [Space(10)] [Colorize] public Timing[] timing;

        [NonSerialized] public uint lastHandle;

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
            return duration.GetRandomValue();
        }

        internal float GetMaxDuration()
        {
            return duration.max * (1 + repeat.count);
        }

        [Serializable]
        public struct RepeatParams
        {
            [Range(0, 99)] public int count;
            public bool forever;
        }
    }
}
