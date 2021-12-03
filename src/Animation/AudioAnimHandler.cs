#region

using Appalachia.Core.Behaviours;
using UnityEngine;

#endregion

namespace Appalachia.Audio.Animation
{
    public sealed class AudioAnimHandler : AppalachiaBehaviour
    {
        #region Fields and Autoproperties

        private AudioAnimEvent[] events;

        #endregion

        #region Event Functions

        protected override void Awake()
        {
            base.Awake();

            var a = GetComponent<Animator>();
            if (a)
            {
                events = a.GetBehaviours<AudioAnimEvent>();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            foreach (var e in events)
            {
                e.KeyOff();
            }
        }

        #endregion
    }
}
