#region

using System;
using Appalachia.Audio.Behaviours;
using Appalachia.Audio.Utilities;
using Appalachia.CI.Constants;
using Appalachia.Utility.Extensions;
using Appalachia.Utility.Strings;
using UnityEngine;

#endregion

namespace Appalachia.Audio.Core
{
    public struct Cue
    {
        #region Static Fields and Autoproperties

        [NonSerialized] private static AppaContext _context;

        #endregion

        #region Fields and Autoproperties

        public AudioEmitter emitter;
        public bool looping;
        public float currentTime;
        public float modVolume;
        public float totalTime;
        public int index;
        public int lastFrame;
        public int repeatCount;
        public int repeatIndex;
        public uint cueHandle;
        public uint keyHandle;

        #endregion

        private static AppaContext Context
        {
            get
            {
                if (_context == null)
                {
                    _context = new AppaContext(typeof(Cue));
                }

                return _context;
            }
        }

        public void KeyOff(float release, EnvelopeMode mode)
        {
            if (Synthesizer.SynthesizerLoggingEnabled)
            {
                Context.Log.Info(
                    ZString.Format(
                        "{0:X4} Sequencer.KeyOff: {1} {2} : {3} {4}",
                        Time.frameCount,
                        emitter.name,
                        emitter.patches[index] ? emitter.patches[index].name : "???",
                        release,
                        mode
                    )
                );
            }

            if (keyHandle != 0)
            {
                Synthesizer.KeyOff(keyHandle, release, mode);
            }

            Reset();
        }

        public bool KeyOn()
        {
            if (Synthesizer.SynthesizerLoggingEnabled)
            {
                Context.Log.Info(
                    ZString.Format(
                        "{0:X4} Sequencer.KeyOn: {1} {2}",
                        Time.frameCount,
                        emitter.name,
                        emitter.patches[index] ? emitter.patches[index].name : "???"
                    )
                );
            }

            if (Randomizer.zeroToOne <= emitter.randomization.chance)
            {
                if (emitter.auxiliary.source)
                {
                    if (!emitter.patches[index].hasTimings)
                    {
                        Synthesizer.KeyOn(
                            emitter.patches[index],
                            emitter.auxiliary.source,
                            0f,
                            emitter.volume
                        );
                    }
                }
                else
                {
                    var listenerTransform = emitter.attachment.useListenerTransform
                        ? Heartbeat.listenerTransform
                        : emitter.attachment.transform
                            ? emitter.attachment.transform
                            : emitter.transform;
                    var randomDistance = emitter.randomization.distance.RandomValue();
                    var soundPosition = Vector3.zero;
                    if (!Mathf.Approximately(randomDistance, 0f))
                    {
                        var randomAngle = Randomizer.plusMinusOne * Mathf.PI * 2f;
                        randomDistance *= emitter.zone.radius;
                        soundPosition.x = Mathf.Sin(randomAngle) * randomDistance;
                        soundPosition.z = Mathf.Cos(randomAngle) * randomDistance;
                    }

                    UpdateModVolume(out var v, 1000f);
                    keyHandle = Synthesizer.KeyOn(
                        out looping,
                        emitter.patches[index],
                        listenerTransform,
                        soundPosition,
                        0f,
                        emitter.volume,
                        v
                    );
                }
            }

            return emitter.patches[index].GetCueInfo(out totalTime, out repeatCount) || looping;
        }

        public void Reset()
        {
            keyHandle = 0;
            currentTime = 0f;
            looping = false;
        }

        public CueStatus Update(float dt)
        {
            var cueStatus = CueStatus.Playing;
            if (lastFrame == Time.frameCount)
            {
                return cueStatus;
            }

            lastFrame = Time.frameCount;
            if (!emitter.paused)
            {
                currentTime += dt;
                if ((totalTime > 0f) && (currentTime >= totalTime))
                {
                    cueStatus = CueStatus.Stopped;
                    if ((repeatCount < 0) || (++repeatIndex < repeatCount))
                    {
                        cueStatus = CueStatus.Repeating;
                    }

                    if (Synthesizer.SynthesizerLoggingEnabled)
                    {
                        Context.Log.Info(
                            ZString.Format(
                                "{0:X4} Cue.Update: {1} {2} {3}/{4} {5}/{6}",
                                Time.frameCount,
                                emitter.name,
                                emitter.patches[index] ? emitter.patches[index].name : "???",
                                currentTime,
                                totalTime,
                                repeatIndex,
                                repeatCount
                            )
                        );
                    }
                }

                if (UpdateModVolume(out var v, dt))
                {
                    if (keyHandle != 0)
                    {
                        Synthesizer.SetModVolume(keyHandle, v);
                    }
                }
            }

            return cueStatus;
        }

        private bool UpdateModVolume(out float vol, float dt)
        {
            var v = 1f;
            var set = false;

            if (emitter.zone)
            {
                if (emitter.zone.hasPeripheralFade)
                {
                    v *= emitter.zone.volumeInfluence;
                    set = true;
                }

                if (emitter.zone.isVolumeExcluded)
                {
                    v *= 1f - emitter.zone.volumeExclusion;
                    set = true;
                }
            }

            if (emitter.isModulated)
            {
                var c = emitter.modulation.custom;
                if (c != null)
                {
                    v *= c.GetCustomModulation();
                    set = true;
                }
                else
                {
                    var t = (currentTime / emitter.modulation.period) * (Mathf.PI * 2f);
                    var x = 1f - ((Mathf.Cos(t) + 1f) * 0.5f);
                    var y = emitter.modulation.volume.RangedValue(x);
                    if (emitter.modulation.inverted)
                    {
                        y = 1f - y;
                    }

                    v *= y;
                    set = true;
                }
            }

            vol = modVolume = Mathf.Lerp(modVolume, v, 2f * dt);
            return set;
        }
    }
}
