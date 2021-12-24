#region

using Appalachia.Core.Objects.Initialization;
using Appalachia.Core.Objects.Root;
using Appalachia.Utility.Async;
using Unity.Profiling;
using UnityEngine;

#endregion

namespace Appalachia.Audio.Animation
{
    public sealed class AudioAnimHandler : AppalachiaBehaviour<AudioAnimHandler>
    {
        #region Fields and Autoproperties

        private AudioAnimEvent[] events;
        private Animator _animator;

        #endregion

        #region Event Functions

        protected override async AppaTask WhenDisabled()

        {
            await base.WhenDisabled();

            foreach (var e in events)
            {
                e.KeyOff();
            }
        }

        #endregion

        protected override async AppaTask Initialize(Initializer initializer)
        {
            using (_PRF_Initialize.Auto())
            {
                await initializer.Do(
                    this,
                    nameof(Animator),
                    _animator == null,
                    () => { _animator = GetComponent<Animator>(); }
                );

                await initializer.Do(
                    this,
                    nameof(AudioAnimEvent),
                    events == null,
                    () => { events = _animator.GetBehaviours<AudioAnimEvent>(); }
                );
            }
        }

        #region Profiling

        private const string _PRF_PFX = nameof(AudioAnimHandler) + ".";

        private static readonly ProfilerMarker _PRF_Initialize =
            new ProfilerMarker(_PRF_PFX + nameof(Initialize));

        #endregion
    }
}
