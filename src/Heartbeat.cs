using Appalachia.Audio.Core;
using Appalachia.Audio.Effects;
using Appalachia.Core.Objects.Initialization;
using Appalachia.Core.Objects.Root;
using Appalachia.Utility.Async;
using Appalachia.Utility.Strings;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Profiling;

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

        #endregion

        #region Event Functions

        protected override async AppaTask Initialize(Initializer initializer)
        {
            await base.Initialize(initializer);
            base.Awake();

            hierarchyTransform = transform;
        }

        protected void Update()
        {
            var dt = Time.deltaTime;
            var tf = Time.frameCount;

            Profiler.BeginSample("Update Zones");
            Zone.UpdateZone(tf);
            Profiler.EndSample();

            Profiler.BeginSample("Update Sequencer");
            Sequencer.Update(dt);
            Profiler.EndSample();

            Profiler.BeginSample("Update Synthesizer");
            Synthesizer.Update(dt);
            Profiler.EndSample();
        }

        protected void LateUpdate()
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

        #endregion

        public void StartRecording(string name)
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

        public int StopRecording()
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
}
