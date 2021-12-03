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
    public struct AudioParameters
    {
        #region Constants and Static Readonly

        public static readonly AudioParameters defaultAudioParameters = new()
        {
            loop = false,
            volume =
                new Vector2
                {
                    x = defaultVolume - defaultVariation, y = defaultVolume + defaultVariation
                },
            pitch =
                new Vector2 { x = defaultPitch - defaultVariation, y = defaultPitch + defaultVariation },
            spatial =
                new SpatialParams
                {
                    blend = defaultBlend,
                    distance = new Vector2 { x = defaultMinDistance, y = defaultMaxDistance },
                    doppler = defaultDoppler
                },
            randomization = new RandomizationParams(),
            occlusion = new OcclusionParams { function = OcclusionFunction.None },
            slapback = new SlapbackParams(),
            runtime = new RuntimeParams { priority = Priority.Normal }
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

        #region Fields and Autoproperties

        [BoxGroup("General")]
        public bool loop;

        [FoldoutGroup("Envelope"), HideLabel, InlineProperty]
        public EnvelopeParams envelope;

        [BoxGroup("General")]
        [PropertyRange(-1, 1)] public float panning;

        [FoldoutGroup("Occlusion"), HideLabel, InlineProperty]
        public OcclusionParams occlusion;

        [FoldoutGroup("Randomization"), HideLabel, InlineProperty]
        public RandomizationParams randomization;

        [FoldoutGroup("Runtime"), HideLabel, InlineProperty]
        public RuntimeParams runtime;

        [FoldoutGroup("Slapback"), HideLabel, InlineProperty]
        public SlapbackParams slapback;

        [FoldoutGroup("Spatial"), HideLabel, InlineProperty]
        public SpatialParams spatial;

        [BoxGroup("General")]
        [MinMaxSlider(0, 10, true)] public Vector2 pitch;

        [BoxGroup("General")]
        [MinMaxSlider(0, 1, true)] public Vector2 volume;

        #endregion

        public float GetPitch()
        {
            return pitch.RandomValue();
        }

        public float GetVolume()
        {
            return volume.RandomValue();
        }

        #region Nested type: EnvelopeParams

        [Serializable]
        public struct EnvelopeParams
        {
            #region Fields and Autoproperties

            [PropertyRange(0, 60)] public float attack;
            [PropertyRange(0, 60)] public float release;

            #endregion
        }

        #endregion

        #region Nested type: OcclusionParams

        [Serializable]
        public struct OcclusionParams
        {
            #region Fields and Autoproperties

            public OcclusionFunction function;

            #endregion
        }

        #endregion

        #region Nested type: RandomizationParams

        [Serializable]
        public struct RandomizationParams
        {
            #region Fields and Autoproperties

            [MinMaxSlider(0, 1, true)] public Vector2 distance;

            #endregion
        }

        #endregion

        #region Nested type: RuntimeParams

        [Serializable]
        public struct RuntimeParams
        {
            #region Fields and Autoproperties

            public Priority priority;

            #endregion
        }

        #endregion

        #region Nested type: SlapbackParams

        [Serializable]
        public struct SlapbackParams
        {
            #region Fields and Autoproperties

            [FormerlySerializedAs("asset")]
            public Patch patch;

            #endregion
        }

        #endregion

        #region Nested type: SpatialParams

        [Serializable]
        public struct SpatialParams
        {
            #region Fields and Autoproperties

            [PropertyRange(0, 1)] public float blend;
            [PropertyRange(0, 5)] public float doppler;
            [MinMaxSlider(0, 1000, true)] public Vector2 distance;

            #endregion
        }

        #endregion
    }
}
