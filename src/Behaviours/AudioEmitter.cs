#region

using System;
using Appalachia.Audio.Core;
using Appalachia.Audio.Scriptables;
using Appalachia.CI.Integration.Assets;
using Appalachia.Core.Objects.Initialization;
using Appalachia.Core.Objects.Root;
using Appalachia.Utility.Async;
using Sirenix.OdinInspector;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Serialization;

#endregion

namespace Appalachia.Audio.Behaviours
{
    [AddComponentMenu(PKG.Menu.Appalachia.Components.Base + nameof(AudioEmitter))]
    public sealed class AudioEmitter : AppalachiaBehaviour<AudioEmitter>
    {
        public enum Controller
        {
            None,
            User,
            Zone
        }

        #region Fields and Autoproperties

        [FoldoutGroup("Attachment"), HideLabel, InlineProperty]
        public AttachmentParams attachment;

        [FoldoutGroup("Auxiliary"), HideLabel, InlineProperty]
        public AuxiliaryParams auxiliary;

        [FoldoutGroup("Execution"), PropertyOrder(-3)]
        public bool autoCue = true;
        
        [FoldoutGroup("Execution"), PropertyOrder(-2)]
        [FormerlySerializedAs("singleShot")] public bool playOnceOnly;

        [ReadOnly, FoldoutGroup("Control")] public Controller controller;

        [FoldoutGroup("Execution"), PropertyOrder(-1)]
        [PropertyRange(0, 1)] public float volume = 1f;

        [FoldoutGroup("Gizmo"), HideLabel, InlineProperty]
        public GizmoParams gizmo = new() { color = Color.red };

        [FoldoutGroup("Modulation"), HideLabel, InlineProperty]
        public ModulationParams modulation = new() { volume = new Vector2 { x = 0.95f, y = 1.05f } };

        [FormerlySerializedAs("assets")]
        public Patch[] patches;
        
        [FoldoutGroup("Randomization"), HideLabel, InlineProperty]
        public RandomizationParams randomization = new() { chance = 1f };
        
        [ReadOnly, FoldoutGroup("Control")]
        public AudioZone zone;
        
        [ReadOnly, FoldoutGroup("Control")]
        internal bool paused;
        
        private bool _wasAlreadyCued;

        private uint[] cueHandles;

        #endregion

        public bool isModulated => (modulation.custom != null) || (modulation.period > 0f);

        #region Event Functions

        protected override async AppaTask WhenDisabled()

        {
            {
                await base.WhenDisabled();
                CueOut();
            }
        }

        #endregion

        [ButtonGroup("Cues"), PropertyOrder(-10)]
        public void CueIn()
        {
            if (!playOnceOnly || !_wasAlreadyCued)
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

                _wasAlreadyCued = true;
            }
        }

        [ButtonGroup("Cues")]
        public void CueOut()
        {
            CueOutWithRelease(0f, EnvelopeMode.None);
        }

        [ButtonGroup("Cues")]
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

        protected override async AppaTask Initialize(Initializer initializer)
        {
            using (_PRF_Initialize.Auto())
            {
                await base.Initialize(initializer);

                if (controller == Controller.None)
                {
                    controller = !zone && !GetComponent<AudioZone>() ? Controller.User : Controller.Zone;
                }

                enabled = controller != Controller.Zone;

                if (autoCue)
                {
                    CueIn();
                }
            }
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
            [PropertyRange(0, 1800)] public float period;

            [MinMaxSlider(0, 2, true)]
            public Vector2 volume;

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

            [PropertyRange(0, 1)] public float chance;

            [MinMaxSlider(0, 1, true)]
            public Vector2 distance;

            #endregion
        }

        #endregion

        #region Profiling

        private const string _PRF_PFX = nameof(AudioEmitter) + ".";

        private static readonly ProfilerMarker _PRF_Initialize =
            new ProfilerMarker(_PRF_PFX + nameof(Initialize));

        private static readonly ProfilerMarker _PRF_OnDisable =
            new ProfilerMarker(_PRF_PFX + nameof(OnDisable));

        #endregion

#if UNITY_EDITOR

        private void OnSceneGUI()
        {
            var t = transform;
            AudioZone z = null;
            for (; t != null; t = t.transform.parent)
            {
                if ((z = t.GetComponent<AudioZone>()) != null)
                {
                    break;
                }
            }

            if (z != null)
            {
                var v = z.transform.position;
                AudioZone.DrawZoneLabelStatic(z, v); // XXX this is drawn multiple times because zone
                var q = z.transform.parent;
                if (q != null)
                {
                    for (int i = 0, n = q.childCount; i < n; ++i)
                    {
                        var u = q.GetChild(i).GetComponents<AudioZone>();
                        for (int j = 0, m = u.Length; j < m; ++j)
                        {
                            if (u[j] && (u[j] != z))
                            {
                                AudioZone.DrawZoneLabelStatic(u[j], u[j].transform.position);
                            }
                        }
                    }
                }
            }
        }

        #region Menu Items

        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Components.Base + nameof(AudioEmitter))]
        private static void CreateAudioEmitter()
        {
            var o = new GameObject("Audio Emitter");
            o.AddComponent<AudioEmitter>();

            var p = UnityEditor.Selection.activeGameObject;
            if ((p != null) && !AssetDatabaseManager.IsMainAsset(p) && !AssetDatabaseManager.IsSubAsset(p))
            {
                o.transform.parent = p.transform;
            }

            UnityEditor.EditorGUIUtility.PingObject(o);
        }

        #endregion

#endif
    }
}
