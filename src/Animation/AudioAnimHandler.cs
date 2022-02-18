#region

using Appalachia.Core.Objects.Initialization;
using Appalachia.Core.Objects.Root;
using Appalachia.Utility.Async;
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

        /// <inheritdoc />
        protected override async AppaTask Initialize(Initializer initializer)
        {
            await base.Initialize(initializer);

            using (_PRF_Initialize.Auto())
            {
                initializer.Do(
                    this,
                    nameof(Animator),
                    _animator == null,
                    () =>
                    {
                        using (_PRF_Initialize.Auto())
                        {
                            _animator = GetComponent<Animator>();
                        }
                    }
                );

                initializer.Do(
                    this,
                    nameof(AudioAnimEvent),
                    events == null,
                    () =>
                    {
                        using (_PRF_Initialize.Auto())
                        {
                            events = _animator.GetBehaviours<AudioAnimEvent>();
                        }
                    }
                );
            }
        }

        /// <inheritdoc />
        protected override async AppaTask WhenDisabled()
        {
            await base.WhenDisabled();

            using (_PRF_WhenDisabled.Auto())
            {
                foreach (var e in events)
                {
                    e.KeyOff();
                }
            }
        }
    }
}
