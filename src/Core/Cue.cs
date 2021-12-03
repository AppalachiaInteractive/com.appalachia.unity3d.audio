#region

using Appalachia.Audio.Behaviours;
using Appalachia.Audio.Utilities;
using Appalachia.Utility.Extensions;
using UnityEngine;

#endregion

namespace Appalachia.Audio.Core
{
    public struct Cue
    {
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

        public void KeyOff(float release, EnvelopeMode mode)
        {
#if SEQUENCER_PARANOIA
        AppaLog.Info(string.Format(
            Time.frameCount.ToString("X4") +
            " Sequencer.KeyOff: {0} {1} : {2} {3}",
            emitter.name, emitter.patches[index] ? emitter.patches[index].name : "???",
            release, mode);
#endif
            if (keyHandle != 0)
            {
                Synthesizer.KeyOff(keyHandle, release, mode);
            }

            Reset();
        }

        public bool KeyOn()
        {
#if SEQUENCER_PARANOIA
        AppaLog.Info(string.Format(
            Time.frameCount.ToString("X4") +
            " Sequencer.KeyOn: {0} {1}",
            emitter.name, emitter.patches[index] ? emitter.patches[index].name : "???");
#endif
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

                    float v;
                    UpdateModVolume(out v, 1000f);
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
#if SEQUENCER_PARANOIA
                AppaLog.Info(string.Format(
                    Time.frameCount.ToString("X4") +
                    " Cue.Update: {0} {1} {2} {3}/{4} {5}/{6}",
                    emitter.name, emitter.patches[index] ? emitter.patches[index].name : "???",
                    s, currentTime, totalTime, repeatIndex, repeatCount);
#endif
                }

                float v;
                if (UpdateModVolume(out v, dt))
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
