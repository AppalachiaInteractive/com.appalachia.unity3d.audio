#region

using System;
using Appalachia.Audio.Utilities;
using Appalachia.CI.Constants;
using UnityEngine;
using UnityEngine.Serialization;

#endregion

namespace Appalachia.Audio.Components
{
    [AddComponentMenu(APPA_MENU.ASM_AppalachiaAudio + nameof(AudioEmitter))]
    public class AudioEmitter : MonoBehaviour
    {
        [Colorize] public AttachmentParams attachment;

        [Colorize] public AuxiliaryParams auxiliary;

        public bool autoCue = true;
        public bool singleShot;

        [HideInInspector] public Controller controller;

        [Range(0, 1)] public float volume = 1f;

        [Colorize] public GizmoParams gizmo = new() {color = Color.red};

        [Colorize]
        public ModulationParams modulation = new() {volume = new MinMaxFloat {min = 0.95f, max = 1.05f}};

        [FormerlySerializedAs("assets")]
        public Patch[] patches;

        [Colorize] public RandomizationParams randomization = new() {chance = 1f};

        internal AudioZone zone;
        internal bool paused;
        private bool cuedOnce;

        private uint[] cueHandles;

        public bool isModulated => (modulation.custom != null) || (modulation.period > 0f);

        public bool IsPaused()
        {
            return paused;
        }

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

        public void SetPaused(bool p)
        {
            paused = p;
        }

        protected void Awake()
        {
            Reset();
        }

        protected void OnDisable()
        {
            CueOut();
        }

        protected void OnEnable()
        {
            if (autoCue)
            {
                CueIn();
            }
        }

        protected void Reset()
        {
            if (controller == Controller.None)
            {
                controller = !zone && !GetComponent<Zone>() ? Controller.User : Controller.Zone;
            }

            enabled = controller != Controller.Zone;
        }

        public enum Controller
        {
            None,
            User,
            Zone
        }

        public interface CustomModulator
        {
            float GetCustomModulation();
        }

        [Serializable]
        public struct AttachmentParams
        {
            public Transform transform;
            public bool useListenerTransform;
        }

        [Serializable]
        public struct AuxiliaryParams
        {
            public AudioSource source;
        }

        [Serializable]
        public struct GizmoParams
        {
            public Color color;
        }

        [Serializable]
        public struct ModulationParams
        {
            [MinMax(0, 2)] public MinMaxFloat volume;
            [Range(0, 1800)] public float period;
            public bool inverted;

            // ReSharper disable once UnassignedField.Global
            internal CustomModulator custom;
        }

        [Serializable]
        public struct RandomizationParams
        {
            [Range(0, 1)] public float chance;
            [MinMax(0, 1)] public MinMaxFloat distance;
        }
    }
}
