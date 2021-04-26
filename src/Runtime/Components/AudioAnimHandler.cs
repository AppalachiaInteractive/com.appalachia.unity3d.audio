#region

using UnityEngine;

#endregion

namespace Internal.Core.Audio
{
    public sealed class AudioAnimHandler : MonoBehaviour
    {
        private AudioAnimEvent[] events;

        private void Awake()
        {
            var a = GetComponent<Animator>();
            if (a)
            {
                events = a.GetBehaviours<AudioAnimEvent>();
            }
        }

        private void OnDisable()
        {
            foreach (var e in events)
            {
                e.KeyOff();
            }
        }
    }
} // Internal.Core.Audio
