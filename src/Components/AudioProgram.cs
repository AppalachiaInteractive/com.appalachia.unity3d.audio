#region

using System;
using Appalachia.Audio.Utilities;
using UnityEngine;
using UnityEngine.Audio;

#endregion

namespace Appalachia.Audio.Components
{
    [Serializable]
    public class AudioProgram
    {
        #region Fields

        [Space(10)] [Colorize] public AudioClipParams[] clips;

        [Space(10)] [Colorize] public AudioMixerGroup mixerGroup;

        [Space(10)]
        [Colorize]
        public AudioParameters audioParameters = AudioParameters.defaultAudioParameters;

        [Colorize] public bool increment = true;

        [Colorize] public bool randomize = true;

        [NonSerialized] public int clipIndex;

        [NonSerialized] public int lastFrame;

        [NonSerialized] public Patch patch;

        [HideInInspector] public WeightedDecay weighted;

        #endregion

        public bool Activate(
            ActivationParams ap
#if UNITY_EDITOR
            ,
            Patch patch
#endif
        )
        {
            var delayed = !Mathf.Approximately(ap.delay, 0f);
            if (delayed ||
                (!randomize && !increment) ||
                (lastFrame != Time.frameCount) ||
                !Application.isPlaying)
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
            if (delayed ||
                (!randomize && !increment) ||
                (lastFrame != Time.frameCount) ||
                !Application.isPlaying)
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

        #region Nested type: AudioClipParams

        [Serializable]
        public struct AudioClipParams
        {
            #region Fields

            public AudioClip clip;
            [Range(-1, 1)] public float gain;

            #endregion
        }

        #endregion
    }
}
