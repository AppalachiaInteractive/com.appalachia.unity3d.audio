using Appalachia.Audio.Components;
using UnityEngine;

namespace Appalachia.Audio.Effects
{
    public sealed class Occlusion : MonoBehaviour
    {
        public AudioParameters.OcclusionParams occlusion;
        public AudioParameters.SpatialParams spatial;
        private AudioHighPassFilter _highPassFilter;
        private AudioLowPassFilter _lowPassFilter;

        private AudioSource _source;
        private float _current;
        private float _target;
        private int _lastFrame;
        private OcclusionSettings _settings;

        public float GetCurrent()
        {
            return _current;
        }

        public float GetTarget()
        {
            return _target;
        }

        private void LateUpdate()
        {
            Test(Time.deltaTime);
        }

        private void OnEnable()
        {
            if (_source == null)
            {
                _source = GetComponent<AudioSource>();
            }

            if (_lowPassFilter == null)
            {
                _lowPassFilter = GetComponent<AudioLowPassFilter>();
            }

            if (_highPassFilter == null)
            {
                _highPassFilter = GetComponent<AudioHighPassFilter>();
            }

            if (_settings == null)
            {
                _settings = OcclusionSettings.instance;
            }

            _lastFrame = -1;
            Test(Mathf.Infinity);
        }

        private void Test(float dt)
        {
            if (_lastFrame != Time.frameCount)
            {
                _lastFrame = Time.frameCount;
                var l = Heartbeat.listenerTransform;
                if (l != null)
                {
                    var lpos = l.position;
                    var d = transform.position - lpos;
                    if (occlusion.function == OcclusionFunction.Distance)
                    {
                        if (Mathf.Approximately(spatial.distance.max - spatial.distance.min, 0f))
                        {
                            _target = d.magnitude >= spatial.distance.max ? 0f : 1f;
                        }
                        else
                        {
                            _target = 1f -
                                      Mathf.Clamp01(
                                          Mathf.Max(0f, d.magnitude - spatial.distance.min) /
                                          (spatial.distance.max - spatial.distance.min)
                                      );
                        }
                    }
                    else if (occlusion.function == OcclusionFunction.Slapback)
                    {
                        if (Mathf.Approximately(spatial.distance.max - spatial.distance.min, 0f))
                        {
                            _target = d.magnitude >= spatial.distance.max ? 0f : 1f;
                        }
                        else
                        {
                            _target = Mathf.Clamp01(
                                Mathf.Max(0f, d.magnitude - spatial.distance.min) /
                                (spatial.distance.max - spatial.distance.min)
                            );
                        }
                    }
                    else if ((occlusion.function == OcclusionFunction.Raycast) &&
                             Physics.Raycast(
                                 lpos,
                                 d.normalized,
                                 d.magnitude,
                                 _settings.layerMask,
                                 QueryTriggerInteraction.Ignore
                             ))
                    {
                        _target = 0f;
                    }
                    else
                    {
                        _target = 1f;
                    }
                }
            }

            _current = Mathf.Lerp(_current, _target, dt * 8f);
            _lowPassFilter.cutoffFrequency = _settings.lowPassRange.min +
                                             ((_settings.lowPassRange.max - _settings.lowPassRange.min) *
                                              _current);
            _highPassFilter.cutoffFrequency = _settings.highPassRange.min +
                                              ((_settings.highPassRange.max - _settings.highPassRange.min) *
                                               (1f - _current));
        }
    }
}
