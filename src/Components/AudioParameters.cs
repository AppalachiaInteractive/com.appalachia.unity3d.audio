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
        public const float defaultVolume = 0.95f;
        public const float defaultPitch = 1f;
        public const float defaultVariation = 0.05f;
        public const float defaultBlend = 1f;
        public const float defaultDoppler = 0.8f;
        public const float defaultMinDistance = 1f;
        public const float defaultMaxDistance = 40f;
        public const float defaultOcclusionMinDistance = 20f;
        public const float defaultOcclusionMaxDistance = 50f;
        public const float defaultSlapbackVolumeAttenuation = 0.4f;
        public const float defaultSlapbackPitchAttenuation = 0.8f;

        public static readonly AudioParameters defaultAudioParameters = new()
        {
            loop = false,
            volume =
                new MinMaxFloat
                {
                    min = defaultVolume - defaultVariation,
                    max = defaultVolume + defaultVariation
                },
            pitch =
                new MinMaxFloat
                {
                    min = defaultPitch - defaultVariation,
                    max = defaultPitch + defaultVariation
                },
            spatial =
                new SpatialParams
                {
                    blend = defaultBlend,
                    distance =
                        new MinMaxFloat
                        {
                            min = defaultMinDistance, max = defaultMaxDistance
                        },
                    doppler = defaultDoppler
                },
            randomization = new RandomizationParams(),
            occlusion = new OcclusionParams {function = OcclusionFunction.None},
            slapback = new SlapbackParams(),
            runtime = new RuntimeParams {priority = Priority.Normal}
        };

        public bool loop;

        [MinMax(0, 1)] public MinMaxFloat volume;

        [MinMax(0, 10)] public MinMaxFloat pitch;

        [Range(-1, 1)] public float panning;

        [Colorize] public EnvelopeParams envelope;

        [Colorize] public SpatialParams spatial;

        [Colorize] public RandomizationParams randomization;

        [Colorize] public OcclusionParams occlusion;

        [Colorize] public SlapbackParams slapback;

        [Colorize] public RuntimeParams runtime;

        public float GetVolume()
        {
            return volume.GetRandomValue();
        }

        public float GetPitch()
        {
            return pitch.GetRandomValue();
        }

        [Serializable]
        public struct EnvelopeParams
        {
            [Range(0, 60)] public float attack;
            [Range(0, 60)] public float release;
        }

        [Serializable]
        public struct SpatialParams
        {
            [Range(0, 1)] public float blend;
            [MinMax(0, 1000)] public MinMaxFloat distance;
            [Range(0, 5)] public float doppler;
        }

        [Serializable]
        public struct RandomizationParams
        {
            [MinMax(0, 1)] public MinMaxFloat distance;
        }

        [Serializable]
        public struct OcclusionParams
        {
            public OcclusionFunction function;
        }

        [Serializable]
        public struct SlapbackParams
        {
            [FormerlySerializedAs("asset")]
            public Patch patch;
        }

        [Serializable]
        public struct RuntimeParams
        {
            public Priority priority;
        }
    }
}
