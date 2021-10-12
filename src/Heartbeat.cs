using Appalachia.Audio.Components;
using Appalachia.Audio.Effects;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Profiling;

namespace Appalachia.Audio
{
    public class Heartbeat : MonoBehaviour
    {
        public static Transform listenerTransform;
        public static Transform playerTransform;

        public AudioMixer audioMixer;
        public string rotationAngleParameter = "Appalachia.Core.Audio Rotation Angle";
        public static Transform hierarchyTransform { get; private set; }

        protected void Awake()
        {
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
                    Debug.LogWarning(
                        "Failed to set audio mixer parameter: " + rotationAngleParameter
                    );
                }
            }
        }

        protected void OnDestroy()
        {
            Sequencer.Reset();
            Synthesizer.Reset();

            if (hierarchyTransform == transform)
            {
                hierarchyTransform = null;
            }
        }

        public void StartRecording(string name)
        {
            if (listenerTransform == null)
            {
                Debug.LogWarning("StartRecording: no listener");
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
                Debug.LogWarning("StopRecording: no listener");
                return -1;
            }

            var r = listenerTransform.GetComponent<RecordToFile>();
            return r ? r.StopRecording() : -1;
        }
    }
} 
