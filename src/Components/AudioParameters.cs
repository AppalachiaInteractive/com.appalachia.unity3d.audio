#region

using System;
using Appalachia.Audio.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

#endregion

namespace Appalachia.Audio.Components
{
    [Serializable]
    public struct AudioParameters
    {
        #region Constants and Static Readonly

        public static readonly AudioParameters defaultAudioParameters = new()
        {
            loop = false,
            volume =
                new FloatRange
                {
                    min = defaultVolume - defaultVariation, max = defaultVolume + defaultVariation
                },
            pitch =
                new FloatRange
                {
                    min = defaultPitch - defaultVariation, max = defaultPitch + defaultVariation
                },
            spatial =
                new SpatialParams
                {
                    blend = defaultBlend,
                    distance = new FloatRange {min = defaultMinDistance, max = defaultMaxDistance},
                    doppler = defaultDoppler
                },
            randomization = new RandomizationParams(),
            occlusion = new OcclusionParams {function = OcclusionFunction.None},
            slapback = new SlapbackParams(),
            runtime = new RuntimeParams {priority = Priority.Normal}
        };

        public const float defaultBlend = 1f;
        public const float defaultDoppler = 0.8f;
        public const float defaultMaxDistance = 40f;
        public const float defaultMinDistance = 1f;
        public const float defaultOcclusionMaxDistance = 50f;
        public const float defaultOcclusionMinDistance = 20f;
        public const float defaultPitch = 1f;
        public const float defaultSlapbackPitchAttenuation = 0.8f;
        public const float defaultSlapbackVolumeAttenuation = 0.4f;
        public const float defaultVariation = 0.05f;
        public const float defaultVolume = 0.95f;

        #endregion

        #region Fields

        public bool loop;

        [Colorize] public EnvelopeParams envelope;

        [Range(-1, 1)] public float panning;

        [FloatRange(0, 10)] public FloatRange pitch;

        [FloatRange(0, 1)] public FloatRange volume;

        [Colorize] public OcclusionParams occlusion;

        [Colorize] public RandomizationParams randomization;

        [Colorize] public RuntimeParams runtime;

        [Colorize] public SlapbackParams slapback;

        [Colorize] public SpatialParams spatial;

        #endregion

        public float GetPitch()
        {
            return pitch.GetRandomValue();
        }

        public float GetVolume()
        {
            return volume.GetRandomValue();
        }

        #region Nested type: EnvelopeParams

        [Serializable]
        public struct EnvelopeParams
        {
            #region Fields

            [Range(0, 60)] public float attack;
            [Range(0, 60)] public float release;

            #endregion
        }

        #endregion

        #region Nested type: OcclusionParams

        [Serializable]
        public struct OcclusionParams
        {
            #region Fields

            public OcclusionFunction function;

            #endregion
        }

        #endregion

        #region Nested type: RandomizationParams

        [Serializable]
        public struct RandomizationParams
        {
            #region Fields

            [FloatRange(0, 1)] public FloatRange distance;

            #endregion
        }

        #endregion

        #region Nested type: RuntimeParams

        [Serializable]
        public struct RuntimeParams
        {
            #region Fields

            public Priority priority;

            #endregion
        }

        #endregion

        #region Nested type: SlapbackParams

        [Serializable]
        public struct SlapbackParams
        {
            #region Fields

            [FormerlySerializedAs("asset")]
            public Patch patch;

            #endregion
        }

        #endregion

        #region Nested type: SpatialParams

        [Serializable]
        public struct SpatialParams
        {
            #region Fields

            [Range(0, 1)] public float blend;
            [Range(0, 5)] public float doppler;
            [FloatRange(0, 1000)] public FloatRange distance;

            #endregion
        }

        #endregion
    }
}
