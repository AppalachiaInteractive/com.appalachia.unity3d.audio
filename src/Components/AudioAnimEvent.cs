#region

using UnityEngine;
using UnityEngine.Serialization;

#endregion

namespace Appalachia.Audio.Components
{
    public sealed class AudioAnimEvent : StateMachineBehaviour
    {
        #region Fields

        [Range(0, 30)] public float delay;

        [FormerlySerializedAs("asset")]
        public Patch patch;

        public Vector3 offset = Vector3.up;
        private uint handle;

        #endregion

        #region Event Functions

        public override void OnStateEnter(Animator animator, AnimatorStateInfo info, int layer)
        {
            KeyOn(animator);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo info, int layer)
        {
            KeyOff();
        }

        #endregion

        public void KeyOff()
        {
            if (handle != 0)
            {
                Synthesizer.KeyOff(handle);
                handle = 0;
            }
        }

        public void KeyOn(Animator a)
        {
            bool looping;
            handle = Synthesizer.KeyOn(out looping, patch, a.transform, offset, delay);
        }
    }
}
