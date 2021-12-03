using System;
using Appalachia.Utility.Interpolation;
using Appalachia.Utility.Interpolation.Modes;
using Appalachia.Utility.Logging;
using Sirenix.OdinInspector;
using UnityEngine.Audio;
using UnityEngine.Playables;

namespace Appalachia.Audio.Playables.MixerGroup
{
    [Serializable]
    public class AudioMixerGroupControlBehaviour : PlayableBehaviour
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

                return $"{_audioMixerGroup.name}_Volume";
            }
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            if (_audioMixerGroup != null)
            {
                var parameterName = AudioMixerGroupParameterName;
                if (!_audioMixerGroup.audioMixer.SetFloat(parameterName, _originalVolume))
                {
                    AppaLog.Error(
                        $"Must initialize audio mixer parameter [{parameterName}] on {nameof(AudioMixerGroup)} [{_audioMixerGroup.name}]."
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

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
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
                AppaLog.Error(
                    $"Must initialize audio mixer parameter [{parameterName}] on {nameof(AudioMixerGroup)} [{_audioMixerGroup.name}]."
                );                
            }
        }
    }
}
