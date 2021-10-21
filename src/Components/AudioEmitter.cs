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
        public enum Controller
        {
            None,
            User,
            Zone
        }

        [FormerlySerializedAs("assets")]
        public Patch[] patches;

        [Range(0, 1)] public float volume = 1f;

        public bool autoCue = true;
        public bool singleShot;

        [HideInInspector] public Controller controller;

        [Colorize] public RandomizationParams randomization = new() {chance = 1f};

        [Colorize]
        public ModulationParams modulation =
            new() {volume = new MinMaxFloat {min = 0.95f, max = 1.05f}};

        [Colorize] public AttachmentParams attachment;

        [Colorize] public AuxiliaryParams auxiliary;

        [Colorize] public GizmoParams gizmo = new() {color = Color.red};
        private bool cuedOnce;

        private uint[] cueHandles;
        internal bool paused;

        internal AudioZone zone;

        public bool isModulated => (modulation.custom != null) || (modulation.period > 0f);

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

        public void SetPaused(bool p)
        {
            paused = p;
        }

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

        public void CueOutInstant()
        {
            CueOutWithRelease(0f, EnvelopeMode.Exact);
        }

        [Serializable]
        public struct RandomizationParams
        {
            [Range(0, 1)] public float chance;
            [MinMax(0, 1)] public MinMaxFloat distance;
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
    }
}
