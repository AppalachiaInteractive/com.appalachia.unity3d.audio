using System;
using System.Collections.Generic;
using System.Reflection;
using Appalachia.Audio.Behaviours;
using Appalachia.Audio.Core;
using Appalachia.Audio.Effects;
using Appalachia.Core.Objects.Initialization;
using Appalachia.Core.Objects.Root;
using Appalachia.Utility.Async;
using Appalachia.Utility.Reflection.Delegated;
using Appalachia.Utility.Reflection.Extensions;
using Appalachia.Utility.Strings;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Audio;

namespace Appalachia.Audio
{
    [ExecuteAlways]
    public sealed class Heartbeat : AppalachiaBehaviour<Heartbeat>
    {
        #region Static Fields and Autoproperties

        public static Transform listenerTransform;
        public static Transform playerTransform;
        public static Transform hierarchyTransform { get; private set; }

        #endregion

        #region Fields and Autoproperties

        public AudioMixer audioMixer;
        public string rotationAngleParameter = "Rotation Angle";

        private List<Action<int>> _zoneUpdateActions;

        #endregion

        #region Event Functions

        private void Update()
        {
            using (_PRF_Update.Auto())
            {
                if (!DependenciesAreReady || !FullyInitialized)
                {
                    return;
                }
                
                var dt = Time.deltaTime;
                var tf = Time.frameCount;

                foreach (var zoneUpdateAction in _zoneUpdateActions)
                {
                    zoneUpdateAction(tf);
                }

                Sequencer.Update(dt);

                Synthesizer.Update(dt);
            }
        }

        private void LateUpdate()
        {
            using (_PRF_LateUpdate.Auto())
            {
                if (playerTransform && audioMixer)
                {
                    var halfRadians = playerTransform.localEulerAngles.y * Mathf.Deg2Rad * 0.5f;
                    if (!audioMixer.SetFloat(rotationAngleParameter, halfRadians))
                    {
                        Context.Log.Warn(
                            ZString.Format("Failed to set audio mixer parameter: {0}", rotationAngleParameter)
                        );
                    }
                }
            }
        }

        #endregion

        public void StartRecording(string name)
        {
            using (_PRF_StartRecording.Auto())
            {
                if (listenerTransform == null)
                {
                    Context.Log.Warn("StartRecording: no listener");
                }
                else
                {
                    var r = listenerTransform.GetComponent<RecordToFile>();
                    if (!r)
                    {
                        r = listenerTransform.gameObject.AddComponent<RecordToFile>();
                    }

                    r.StartRecording(name);
                }
            }
        }

        public int StopRecording()
        {
            using (_PRF_StopRecording.Auto())
            {
                if (listenerTransform == null)
                {
                    Context.Log.Warn("StopRecording: no listener");
                    return -1;
                }

                var r = listenerTransform.GetComponent<RecordToFile>();
                return r ? r.StopRecording() : -1;
            }
        }

        protected override async AppaTask Initialize(Initializer initializer)
        {
            await base.Initialize(initializer);

            hierarchyTransform = transform;

            var zoneTypes = typeof(Zone<>).GetAllConcreteInheritors();

            _zoneUpdateActions = new List<Action<int>>();

            foreach (var zoneType in zoneTypes)
            {
                var routine = StaticRoutine.CreateDelegate<int>(zoneType, "UpdateZone", BindingFlags.Static);

                _zoneUpdateActions.Add(routine);
            }
        }

        protected override async AppaTask WhenDestroyed()
        {
            await base.WhenDestroyed();

            Sequencer.Reset();
            Synthesizer.Reset();

            if (hierarchyTransform == transform)
            {
                hierarchyTransform = null;
            }
        }

        #region Profiling

        private const string _PRF_PFX = nameof(Heartbeat) + ".";

        private static readonly ProfilerMarker _PRF_LateUpdate =
            new ProfilerMarker(_PRF_PFX + nameof(LateUpdate));

        private static readonly ProfilerMarker _PRF_StartRecording =
            new ProfilerMarker(_PRF_PFX + nameof(StartRecording));

        private static readonly ProfilerMarker _PRF_StopRecording =
            new ProfilerMarker(_PRF_PFX + nameof(StopRecording));

        private static readonly ProfilerMarker _PRF_Update = new ProfilerMarker(_PRF_PFX + nameof(Update));

        #endregion
    }
}
