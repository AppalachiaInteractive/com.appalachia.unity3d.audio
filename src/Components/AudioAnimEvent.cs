#region

using UnityEngine;
using UnityEngine.Serialization;

#endregion

namespace Appalachia.Audio.Components
{
    public sealed class AudioAnimEvent : StateMachineBehaviour
    {
        [FormerlySerializedAs("asset")]
        public Patch patch;

        [Range(0, 30)] public float delay;
        public Vector3 offset = Vector3.up;
        private uint handle;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo info, int layer)
        {
            KeyOn(animator);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo info, int layer)
        {
            KeyOff();
        }

        public void KeyOn(Animator a)
        {
            bool looping;
            handle = Synthesizer.KeyOn(out looping, patch, a.transform, offset, delay);
        }

        public void KeyOff()
        {
            if (handle != 0)
            {
                Synthesizer.KeyOff(handle);
                handle = 0;
            }
        }
    }
} 
