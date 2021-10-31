#region

using System;
using Appalachia.Audio.Utilities;
using Appalachia.Core.Scriptables;
using UnityEngine;

#endregion

namespace Appalachia.Audio.Components
{
    public sealed class Patch : AppalachiaObject<Patch>
    {
        [Space(10)] [Colorize] public AudioProgram program;

        [Space(10)] [Colorize] public AudioSequence sequence;

        [NonSerialized] public bool hasTimings;

        public bool Activate(ActivationParams ap)
        {
            return hasTimings
                ? sequence.Activate(ap)
                : program.Activate(
                    ap
#if UNITY_EDITOR
                    ,
                    this
#endif
                );
        }

        public bool Activate(ActivationParams ap, AudioParameters.EnvelopeParams ep)
        {
            return program.Activate(
                ap,
                ep
#if UNITY_EDITOR
                ,
                this
#endif
            );
        }

        public bool GetCueInfo(out float duration, out int repeats)
        {
            if (hasTimings)
            {
                return sequence.GetCueInfo(out duration, out repeats);
            }

            duration = 0f;
            repeats = 0;
            return false;
        }

        public int GetClipIndex()
        {
            return !program.randomize ? program.clipIndex : -1;
        }

        public void SetClipIndex(int index)
        {
            if (!program.randomize)
            {
                program.clipIndex = index;
            }
        }

        internal float GetMaxDuration()
        {
            return Mathf.Max(program.GetMaxDuration(), sequence.GetMaxDuration());
        }

        private void OnEnable()
        {
            if (program != null)
            {
                program.patch = this;
                program.Initialize();
            }

            if (sequence != null)
            {
                sequence.patch = this;
                hasTimings = (sequence.timing != null) && (sequence.timing.Length > 0);
            }
        }
    }
}
