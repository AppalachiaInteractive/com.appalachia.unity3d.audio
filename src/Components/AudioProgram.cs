#region

using System;
using Appalachia.Core.Audio.Utilities;
using UnityEngine;
using UnityEngine.Audio;

#endregion

namespace Appalachia.Core.Audio.Components
{
    [Serializable]
    public class AudioProgram
    {
        [Space(10)] [Colorize] public AudioClipParams[] clips;

        [Colorize] public bool randomize = true;

        [Colorize] public bool increment = true;

        [HideInInspector] public WeightedDecay weighted;

        [Space(10)] [Colorize] public AudioMixerGroup mixerGroup;

        [Space(10)] [Colorize] public AudioParameters audioParameters = AudioParameters.defaultAudioParameters;

        [NonSerialized] public int lastFrame;

        [NonSerialized] public int clipIndex;

        [NonSerialized] public Patch patch;

        public void Initialize()
        {
            lastFrame = -1;
            clipIndex = 0;
        }

        internal float GetMaxDuration()
        {
            var n = 0f;
            foreach (var c in clips)
            {
                n = Mathf.Max(n, c.clip.length);
            }

            return n;
        }

        public AudioClip GetClip(out float gain)
        {
            if (randomize)
            {
                return GetClip(Randomizer.zeroToOne, out gain);
            }

            var i = clipIndex;
            var c = clips[i];
            if (increment)
            {
                clipIndex = (clipIndex + 1) % clips.Length;
            }

            gain = c.gain;
            return c.clip;
        }

        public AudioClip GetClip(float q, out float gain)
        {
            return GetClipAt(clipIndex = weighted.Draw(q, clips.Length), out gain);
        }

        public AudioClip GetClipAt(int i, out float gain)
        {
            var c = clips[i];
            gain = c.gain;
            return c.clip;
        }

        public bool Activate(
            ActivationParams ap
#if UNITY_EDITOR
            ,
            Patch patch
#endif
        )
        {
            var delayed = !Mathf.Approximately(ap.delay, 0f);
            if (delayed || (!randomize && !increment) || (lastFrame != Time.frameCount) || !Application.isPlaying)
            {
                if (!delayed)
                {
                    lastFrame = Time.frameCount;
                }

                return Synthesizer.Activate(
                    this,
                    ap
#if UNITY_EDITOR
                    ,
                    patch
#endif
                );
            }

            return false;
        }

        public bool Activate(
            ActivationParams ap,
            AudioParameters.EnvelopeParams eo
#if UNITY_EDITOR
            ,
            Patch patch
#endif
        )
        {
            var delayed = !Mathf.Approximately(ap.delay, 0f);
            if (delayed || (!randomize && !increment) || (lastFrame != Time.frameCount) || !Application.isPlaying)
            {
                if (!delayed)
                {
                    lastFrame = Time.frameCount;
                }

                return Synthesizer.Activate(
                    this,
                    eo,
                    ap
#if UNITY_EDITOR
                    ,
                    patch
#endif
                );
            }

            return false;
        }

        [Serializable]
        public struct AudioClipParams
        {
            public AudioClip clip;
            [Range(-1, 1)] public float gain;
        }
    }
} // Appalachia.Core.Audio
