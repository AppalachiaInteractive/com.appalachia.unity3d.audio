#region

using UnityEngine;

#endregion

namespace Appalachia.Audio.Components
{
    public sealed class AudioAnimHandler : MonoBehaviour
    {
        #region Fields and Autoproperties

        private AudioAnimEvent[] events;

        #endregion

        #region Event Functions

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

        #endregion
    }
}
