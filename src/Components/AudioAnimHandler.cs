#region

using UnityEngine;

#endregion

namespace Appalachia.Audio.Components
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
}
