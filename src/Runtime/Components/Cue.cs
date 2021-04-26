#region

using UnityEngine;

#endregion

namespace Internal.Core.Audio
{
    public struct Cue
    {
        public AudioEmitter emitter;
        public int index;
        public uint cueHandle;
        public uint keyHandle;
        public int lastFrame;
        public float modVolume;
        public float currentTime;
        public float totalTime;
        public int repeatIndex;
        public int repeatCount;
        public bool looping;

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
                    var y = emitter.modulation.volume.GetRangedValue(x);
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

        public CueStatus Update(float dt)
        {
            var s = CueStatus.Playing;
            if (lastFrame == Time.frameCount)
            {
                return s;
            }

            lastFrame = Time.frameCount;
            if (!emitter.paused)
            {
                currentTime += dt;
                if ((totalTime > 0f) && (currentTime >= totalTime))
                {
                    s = CueStatus.Stopped;
                    if ((repeatCount < 0) || (++repeatIndex < repeatCount))
                    {
                        s = CueStatus.Repeating;
                    }
#if SEQUENCER_PARANOIA
                Debug.LogFormat(
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

            return s;
        }

        public bool KeyOn()
        {
#if SEQUENCER_PARANOIA
        Debug.LogFormat(
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
                        Synthesizer.KeyOn(emitter.patches[index], emitter.auxiliary.source, 0f, emitter.volume);
                    }
                }
                else
                {
                    var t = emitter.attachment.useListenerTransform
                        ? Heartbeat.listenerTransform
                        : emitter.attachment.transform
                            ? emitter.attachment.transform
                            : emitter.transform;
                    var r = emitter.randomization.distance.GetRandomValue();
                    var p = Vector3.zero;
                    if (!Mathf.Approximately(r, 0f))
                    {
                        var a = Randomizer.plusMinusOne * Mathf.PI * 2f;
                        r *= emitter.zone.radius;
                        p.x = Mathf.Sin(a) * r;
                        p.z = Mathf.Cos(a) * r;
                    }

                    float v;
                    UpdateModVolume(out v, 1000f);
                    keyHandle = Synthesizer.KeyOn(out looping, emitter.patches[index], t, p, 0f, emitter.volume, v);
                }
            }

            return emitter.patches[index].GetCueInfo(out totalTime, out repeatCount) || looping;
        }

        public void KeyOff(float release, EnvelopeMode mode)
        {
#if SEQUENCER_PARANOIA
        Debug.LogFormat(
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

        public void Reset()
        {
            keyHandle = 0;
            currentTime = 0f;
            looping = false;
        }
    }
}
