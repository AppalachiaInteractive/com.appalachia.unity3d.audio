using Appalachia.Audio.Core;
using Appalachia.Audio.Scriptables;
using Appalachia.Core.Attributes;
using Appalachia.Core.Objects.Initialization;
using Appalachia.Core.Objects.Root;
using Appalachia.Utility.Async;
using Unity.Profiling;
using UnityEngine;

namespace Appalachia.Audio.Effects
{
    [CallStaticConstructorInEditor]
    public sealed class Occlusion : AppalachiaBehaviour<Occlusion>
    {
        static Occlusion()
        {
            RegisterDependency<OcclusionSettings>(i => _occlusionSettings = i);
        }

        #region Static Fields and Autoproperties

        private static OcclusionSettings _occlusionSettings;

        #endregion

        #region Fields and Autoproperties

        public AudioParameters.OcclusionParams occlusion;
        public AudioParameters.SpatialParams spatial;
        private AudioHighPassFilter _highPassFilter;
        private AudioLowPassFilter _lowPassFilter;

        private AudioSource _source;
        private float _current;
        private float _target;
        private int _lastFrame;

        #endregion

        #region Event Functions

        private void LateUpdate()
        {
            using (_PRF_LateUpdate.Auto())
            {
                Test(Time.deltaTime);
            }
        }

        #endregion

        public float GetCurrent()
        {
            return _current;
        }

        public float GetTarget()
        {
            return _target;
        }

        protected override async AppaTask Initialize(Initializer initializer)
        {
            await base.Initialize(initializer);

            using (_PRF_Initialize.Auto())
            {
                _source = initializer.GetOrCreate(this,         _source);
                _lowPassFilter = initializer.GetOrCreate(this,  _lowPassFilter);
                _highPassFilter = initializer.GetOrCreate(this, _highPassFilter);

                _lastFrame = -1;
                Test(Mathf.Infinity);
            }
        }

        private void Test(float dt)
        {
            using (_PRF_Test.Auto())
            {
                if (!OcclusionSettings.IsInstanceAvailable)
                {
                    return;
                }

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
                            if (Mathf.Approximately(spatial.distance.y - spatial.distance.x, 0f))
                            {
                                _target = d.magnitude >= spatial.distance.y ? 0f : 1f;
                            }
                            else
                            {
                                _target = 1f -
                                          Mathf.Clamp01(
                                              Mathf.Max(0f, d.magnitude - spatial.distance.x) /
                                              (spatial.distance.y - spatial.distance.x)
                                          );
                            }
                        }
                        else if (occlusion.function == OcclusionFunction.Slapback)
                        {
                            if (Mathf.Approximately(spatial.distance.y - spatial.distance.x, 0f))
                            {
                                _target = d.magnitude >= spatial.distance.y ? 0f : 1f;
                            }
                            else
                            {
                                _target = Mathf.Clamp01(
                                    Mathf.Max(0f, d.magnitude - spatial.distance.x) /
                                    (spatial.distance.y - spatial.distance.x)
                                );
                            }
                        }
                        else if ((occlusion.function == OcclusionFunction.Raycast) &&
                                 Physics.Raycast(
                                     lpos,
                                     d.normalized,
                                     d.magnitude,
                                     _occlusionSettings.layerMask,
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
                _lowPassFilter.cutoffFrequency = _occlusionSettings.lowPassRange.x +
                                                 ((_occlusionSettings.lowPassRange.y -
                                                   _occlusionSettings.lowPassRange.x) *
                                                  _current);
                _highPassFilter.cutoffFrequency = _occlusionSettings.highPassRange.x +
                                                  ((_occlusionSettings.highPassRange.y -
                                                    _occlusionSettings.highPassRange.x) *
                                                   (1f - _current));
            }
        }

        #region Profiling

        private static readonly ProfilerMarker _PRF_Test = new ProfilerMarker(_PRF_PFX + nameof(Test));

        #endregion
    }
}
