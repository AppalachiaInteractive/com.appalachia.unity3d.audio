#region

using System;
using Appalachia.Audio.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

#endregion

namespace Appalachia.Audio.Components
{
    [AddComponentMenu(PKG.Menu.Appalachia.Components.Base + nameof(AudioEmitter))]
    public class AudioEmitter : MonoBehaviour
    {
        public enum Controller
        {
            None,
            User,
            Zone
        }

        #region Fields and Autoproperties

        [Colorize] public AttachmentParams attachment;

        [Colorize] public AuxiliaryParams auxiliary;

        public bool autoCue = true;
        public bool singleShot;

        [HideInInspector] public Controller controller;

        [Range(0, 1)] public float volume = 1f;

        [Colorize] public GizmoParams gizmo = new() {color = Color.red};

        [Colorize]
        public ModulationParams modulation = new() {volume = new FloatRange {min = 0.95f, max = 1.05f}};

        [FormerlySerializedAs("assets")]
        public Patch[] patches;

        [Colorize] public RandomizationParams randomization = new() {chance = 1f};

        internal AudioZone zone;
        internal bool paused;
        private bool cuedOnce;

        private uint[] cueHandles;

        #endregion

        public bool isModulated => (modulation.custom != null) || (modulation.period > 0f);

        #region Event Functions

        protected void Awake()
        {
            Reset();
        }

        protected void Reset()
        {
            if (controller == Controller.None)
            {
                controller = !zone && !GetComponent<Zone>() ? Controller.User : Controller.Zone;
            }

            enabled = controller != Controller.Zone;
        }

        protected void OnEnable()
        {
            if (autoCue)
            {
                CueIn();
            }
        }

        protected void OnDisable()
        {
            CueOut();
        }

        #endregion

        public void CueIn()
        {
            if (!singleShot || !cuedOnce)
            {
                CueOut();
                if ((cueHandles == null) || (cueHandles.Length != patches.Length))
                {
                    cueHandles = new uint[patches.Length];
                }

                for (int i = 0, n = cueHandles.Length; i < n; ++i)
                {
                    cueHandles[i] = Sequencer.CueIn(this, i);
                }

                cuedOnce = true;
            }
        }

        public void CueOut()
        {
            CueOutWithRelease(0f, EnvelopeMode.None);
        }

        public void CueOutInstant()
        {
            CueOutWithRelease(0f, EnvelopeMode.Exact);
        }

        public void CueOutWithRelease(float release, EnvelopeMode mode)
        {
            if (cueHandles != null)
            {
                for (int i = 0, n = cueHandles.Length; i < n; ++i)
                {
                    Sequencer.CueOut(cueHandles[i], release, mode);
                    cueHandles[i] = 0;
                }
            }
        }

        public bool IsPaused()
        {
            return paused;
        }

        public void SetPaused(bool p)
        {
            paused = p;
        }

        #region Nested type: AttachmentParams

        [Serializable]
        public struct AttachmentParams
        {
            #region Fields and Autoproperties

            public bool useListenerTransform;
            public Transform transform;

            #endregion
        }

        #endregion

        #region Nested type: AuxiliaryParams

        [Serializable]
        public struct AuxiliaryParams
        {
            #region Fields and Autoproperties

            public AudioSource source;

            #endregion
        }

        #endregion

        #region Nested type: CustomModulator

        public interface CustomModulator
        {
            float GetCustomModulation();
        }

        #endregion

        #region Nested type: GizmoParams

        [Serializable]
        public struct GizmoParams
        {
            #region Fields and Autoproperties

            public Color color;

            #endregion
        }

        #endregion

        #region Nested type: ModulationParams

        [Serializable]
        public struct ModulationParams
        {
            #region Fields and Autoproperties

            public bool inverted;
            [Range(0, 1800)] public float period;
            [FloatRange(0, 2)] public FloatRange volume;

            // ReSharper disable once UnassignedField.Global
            internal CustomModulator custom;

            #endregion
        }

        #endregion

        #region Nested type: RandomizationParams

        [Serializable]
        public struct RandomizationParams
        {
            #region Fields and Autoproperties

            [Range(0, 1)] public float chance;
            [FloatRange(0, 1)] public FloatRange distance;

            #endregion
        }

        #endregion
    }
}
