using System;
using Appalachia.Core.Objects.Initialization;
using Appalachia.Core.Objects.Root;
using Appalachia.Utility.Async;
using Appalachia.Utility.Interpolation;
using Appalachia.Utility.Interpolation.Modes;
using Appalachia.Utility.Strings;
using Sirenix.OdinInspector;
using UnityEngine.Audio;
using UnityEngine.Playables;

namespace Appalachia.Audio.Playables.MixerGroup
{
    [Serializable]
    public class AudioMixerGroupControlBehaviour : AppalachiaPlayable<AudioMixerGroupControlBehaviour>
    {
        #region Fields and Autoproperties

        [PropertyRange(-80f, 20f), SuffixLabel("dB"), PropertyOrder(-5)]
        public float endVolume;

        [PropertyRange(-80f, 20f), SuffixLabel("dB"), PropertyOrder(-10)]
        public float startVolume;

        private AudioMixerGroup _audioMixerGroup;

        private bool _storedOriginals;
        private float _originalVolume;
        private IInterpolationMode _interpolation;

        #endregion

        [ShowInInspector, PropertyOrder(5)]
        public InterpolationMode fadeMode =>
            startVolume > endVolume ? InterpolationMode.EaseOutQuad : InterpolationMode.EaseInQuad;

        [ShowInInspector, PropertyOrder(10)]
        private string AudioMixerGroupParameterName
        {
            get
            {
                if (_audioMixerGroup == null)
                {
                    return null;
                }

                return ZString.Format("{0}_Volume", _audioMixerGroup.name);
            }
        }

        /// <inheritdoc />
        protected override async AppaTask Initialize(Initializer initializer)
        {
            await AppaTask.CompletedTask;
        }

        /// <inheritdoc />
        protected override void OnPause(Playable playable, FrameData info)
        {
        }

        /// <inheritdoc />
        protected override void OnPlay(Playable playable, FrameData info)
        {
        }

        /// <inheritdoc />
        protected override void Update(Playable playable, FrameData info, object playerData)
        {
            using (_PRF_Update.Auto())
            {
                _audioMixerGroup = playerData as AudioMixerGroup;

                if (_audioMixerGroup == null)
                {
                    return;
                }

                var parameterName = AudioMixerGroupParameterName;

                if (!_storedOriginals)
                {
                    _audioMixerGroup.audioMixer.GetFloat(parameterName, out _originalVolume);
                    _storedOriginals = true;
                }

                var currentTime = playable.GetTime();
                var duration = playable.GetDuration();
                var percentage = currentTime / duration;

                if ((_interpolation == null) || (_interpolation.mode != fadeMode))
                {
                    _interpolation = InterpolatorFactory.GetInterpolator(fadeMode);
                }

                var fadeValue = _interpolation.Interpolate(startVolume, endVolume, (float)percentage);

                if (!_audioMixerGroup.audioMixer.SetFloat(parameterName, fadeValue))
                {
                    Context.Log.Error(
                        ZString.Format(
                            "Must initialize audio mixer parameter [{0}] on {1} [{2}].",
                            parameterName,
                            nameof(AudioMixerGroup),
                            _audioMixerGroup.name
                        )
                    );
                }
            }
        }

        /// <inheritdoc />
        protected override void WhenDestroyed(Playable playable)
        {
            using (_PRF_WhenDestroyed.Auto())
            {
                if (_audioMixerGroup != null)
                {
                    var parameterName = AudioMixerGroupParameterName;
                    if (!_audioMixerGroup.audioMixer.SetFloat(parameterName, _originalVolume))
                    {
                        Context.Log.Error(
                            ZString.Format(
                                "Must initialize audio mixer parameter [{0}] on {1} [{2}].",
                                parameterName,
                                nameof(AudioMixerGroup),
                                _audioMixerGroup.name
                            )
                        );
                    }
                }

                startVolume = 0f;
                endVolume = 0f;
                _interpolation = null;
                _audioMixerGroup = null;
                _storedOriginals = false;
                _originalVolume = 0.0f;
            }
        }

        /// <inheritdoc />
        protected override void WhenStarted(Playable playable)
        {
        }

        /// <inheritdoc />
        protected override void WhenStopped(Playable playable)
        {
        }
    }
}
