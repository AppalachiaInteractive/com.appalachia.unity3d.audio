using Appalachia.Audio.Core;
using Appalachia.Audio.Scriptables;
using Appalachia.Core.Objects.Initialization;
using Appalachia.Core.Objects.Root;
using Appalachia.Utility.Async;
using Unity.Profiling;
using UnityEngine;

namespace Appalachia.Audio.Effects
{
    public sealed class Occlusion : AppalachiaBehaviour<Occlusion>
    {
        // [CallStaticConstructorInEditor] should be added to the class (initsingletonattribute)
        static Occlusion()
        {
            RegisterDependency<OcclusionSettings>(i => _occlusionSettings = i);
        }

        private static OcclusionSettings _occlusionSettings;
        
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
            using (_PRF_Initialize.Auto())
            {
                await base.Initialize(initializer);

                _source = await initializer.GetOrCreate<AudioSource>(this);
                _lowPassFilter = await initializer.GetOrCreate<AudioLowPassFilter>(this);
                _highPassFilter = await initializer.GetOrCreate<AudioHighPassFilter>(this);

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

        private const string _PRF_PFX = nameof(Occlusion) + ".";

        private static readonly ProfilerMarker _PRF_LateUpdate =
            new ProfilerMarker(_PRF_PFX + nameof(LateUpdate));

        private static readonly ProfilerMarker _PRF_Initialize =
            new ProfilerMarker(_PRF_PFX + nameof(Initialize));

        private static readonly ProfilerMarker _PRF_Test = new ProfilerMarker(_PRF_PFX + nameof(Test));

        #endregion
    }
}
