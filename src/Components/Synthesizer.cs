// #define SYNTHESIZER_PARANOIA

#region

using System;
using System.Collections.Generic;
using System.Reflection;
using Appalachia.Audio.Effects;
using Appalachia.Audio.Utilities;
using Appalachia.Utility.Logging;
using UnityEngine;
using UnityEngine.Audio;

#endregion

namespace Appalachia.Audio.Components
{
    public static class Synthesizer
    {
        #region Fields

#if UNITY_EDITOR
        public static int sourceIndex;
#endif
        public static List<ActiveSource> activeSources0 = new(64);
        public static List<ActiveSource> activeSources1 = new(64);

        public static Stack<SourceInfo> freeSources = new(64);
        public static uint activeHandle;
        private static float _masterVolume = 1f;

        #endregion

        public static float masterVolume
        {
            get => _masterVolume;
            set => _masterVolume = Mathf.Clamp01(value);
        }

        public static bool IsKeyedOn(uint handle)
        {
            if (handle == 0)
            {
                AppaLog.Error("IsKeyedOn: bad handle");
                return false;
            }

            foreach (var i in activeSources0)
            {
                if (i.handle == handle)
                {
                    return true;
                }
            }

            return false;
        }

        public static void KeyOff(uint handle, float release = 0f, EnvelopeMode mode = EnvelopeMode.None)
        {
            if (handle == 0)
            {
                AppaLog.Error("KeyOff: bad handle");
                return;
            }

            using (var enumerator = activeSources0.GetEnumerator())
            {
                for (var x = enumerator; x.MoveNext();)
                {
                    var z = x.Current;
                    if (z.handle == handle)
                    {
#if SYNTHESIZER_PARANOIA
                AppaLog.Info(string.Format(
                    Time.frameCount.ToString("X4") +
                    " Synthesizer.KeyOff: {0} ({1}) : {2} {3}",
                    z.info.audioSource.clip.name, z.info.audioSource.name,
                    release, mode);
#endif
                        switch (mode)
                        {
                            case EnvelopeMode.Exact:
                                z.envelope.SetRelease(release);
                                break;
                            case EnvelopeMode.Min:
                                z.envelope.SetRelease(Mathf.Min(z.envelope.releaseTime, release));
                                break;
                            case EnvelopeMode.Max:
                                z.envelope.SetRelease(Mathf.Max(z.envelope.releaseTime, release));
                                break;
                        }

                        z.keyOff = true;
                    }

                    activeSources1.Add(z);
                }
            }

            Swap(ref activeSources0, ref activeSources1);
            activeSources1.Clear();
        }

        public static uint KeyOn(
            AudioMixerGroup g,
            AudioClip c,
            AudioParameters p,
            Transform t = null,
            Vector3 pos = new(),
            float delay = 0f,
            float volume = 1f,
            float modVolume = 1f)
        {
            if (c == null)
            {
                AppaLog.Error("KeyOn: missing audio clip reference");
                return 0;
            }

            var handle = GetNextHandle();
            Activate(
                g,
                c,
                p,
                new ActivationParams
                {
                    transform = t,
                    position = pos,
                    delay = delay,
                    volume = volume,
                    modVolume = modVolume,
                    handle = handle
                }
#if UNITY_EDITOR
                ,
                null
#endif
            );
            return handle;
        }

        public static uint KeyOn(
            out bool looping,
            Patch patch,
            Transform t = null,
            Vector3 pos = new(),
            float delay = 0f,
            float volume = 1f,
            float modVolume = 1f)
        {
            if (patch == null)
            {
                AppaLog.Error("KeyOn: missing patch reference");
                looping = false;
                return 0;
            }

            var handle = GetNextHandle();
            looping = patch.Activate(
                new ActivationParams
                {
                    transform = t,
                    position = pos,
                    delay = delay,
                    volume = volume,
                    modVolume = modVolume,
                    handle = handle
                }
            );
            return handle;
        }

        public static uint KeyOn(
            out bool looping,
            Patch patch,
            AudioParameters.EnvelopeParams ep,
            Transform t = null,
            Vector3 pos = new(),
            float delay = 0f,
            float volume = 1f,
            float modVolume = 1f)
        {
            if (patch == null)
            {
                AppaLog.Error("KeyOn: missing patch reference");
                looping = false;
                return 0;
            }

            var handle = GetNextHandle();
            looping = patch.Activate(
                new ActivationParams
                {
                    transform = t,
                    position = pos,
                    delay = delay,
                    volume = volume,
                    modVolume = modVolume,
                    handle = handle
                },
                ep
            );
            return handle;
        }

        public static void KeyOn(Patch patch, AudioSource src, float delay = 0f, float volume = 1f)
        {
            if (patch == null)
            {
                AppaLog.Error("KeyOn: missing patch reference");
                return;
            }

            Activate(patch, src, delay, volume);
        }

        public static void Reset()
        {
            StopAll();
            freeSources.Clear();
            activeSources0.Clear();
            activeSources1.Clear();
        }

        public static void Stop(uint handle)
        {
            if (handle == 0)
            {
                AppaLog.Error("Stop: bad handle");
                return;
            }

            using (var enumerator = activeSources0.GetEnumerator())
            {
                for (var x = enumerator; x.MoveNext();)
                {
                    var z = x.Current;
                    if ((z.handle == handle) && z.info.audioSource)
                    {
#if SYNTHESIZER_PARANOIA
                AppaLog.Info(string.Format(
                    Time.frameCount.ToString("X4") +
                    " Synthesizer.Update: {0} ({1}) : stopped by envelope",
                    z.info.audioSource.clip.name, z.info.audioSource.name);
#endif
                        z.info.audioSource.Stop();
                    }
                }
            }
        }

        public static void StopAll()
        {
#if SYNTHESIZER_PARANOIA
        AppaLog.Info(string.Format("Synthesizer.StopAll");
#endif
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                StopAllClips();
                return;
            }
#endif
            using (var enumerator = activeSources0.GetEnumerator())
            {
                for (var x = enumerator; x.MoveNext();)
                {
                    var z = x.Current;
                    if (z.info.audioSource)
                    {
                        z.info.audioSource.Stop();
                    }
                }
            }
        }

        internal static bool Activate(
            AudioMixerGroup g,
            AudioClip c,
            AudioParameters p,
            ActivationParams ap
#if UNITY_EDITOR
            ,
            Patch patch
#endif
        )
        {
            var r = p.randomization.distance.GetRandomValue();
            if (!Mathf.Approximately(r, 0f))
            {
                var a = Randomizer.plusMinusOne * Mathf.PI;
                r *= p.spatial.distance.max - p.spatial.distance.min;
                r += p.spatial.distance.min;
                ap.position.x += Mathf.Sin(a) * r;
                ap.position.z += Mathf.Cos(a) * r;
            }

            var pos2 = ap.position;
            if (ap.transform != null)
            {
                pos2 += ap.transform.position;
            }

            var looping = ActivateInternal(
                g,
                c,
                p,
                ap,
                pos2,
                1f
#if UNITY_EDITOR
                ,
                patch
#endif
            );
            if (!looping && (p.occlusion.function != OcclusionFunction.None) && (p.slapback.patch != null))
            {
                AudioSlapback s;
                Vector3 pos3, d3;
                if ((bool) (s = AudioSlapback.FindClosest(pos2, out pos3, out d3)))
                {
                    var patch2 = p.slapback.patch;
                    var p2 = patch2.program.audioParameters;

                    var lpos = Heartbeat.listenerTransform.position;
                    var d = pos3 - lpos;
                    var pos4 = pos3 + (d.normalized * s.radius);
                    var dt = s.radius / OcclusionSettings.instance.speedOfSound;

                    p2.randomization.distance.min = 0f;
                    p2.randomization.distance.max = 0f;
                    p2.occlusion.function = OcclusionFunction.Slapback;

                    float gain;
                    var clip = patch2.program.GetClip(out gain);
                    if (!clip)
                    {
                        AppaLog.Error("Activate: Null AudioClip from patch: " + patch2, patch2);
                    }

                    Activate(
                        patch2.program.mixerGroup,
                        clip,
                        p2,
                        new ActivationParams
                        {
                            transform = null,
                            position = pos4,
                            delay = ap.delay + dt,
                            volume = ap.volume * (1f + gain),
                            modVolume = ap.modVolume,
                            handle = ap.handle
                        }
#if UNITY_EDITOR
                        ,
                        patch2
#endif
                    );
                }
            }

            return looping;
        }

        internal static bool Activate(Patch patch, AudioSource src, float delay, float volume)
        {
            var p = patch.program.audioParameters;
            float gain;
            var clip = patch.program.GetClip(out gain);
            if (!clip)
            {
                AppaLog.Error("Activate: Null AudioClip from patch: " + patch, patch);
            }

            var looping = ActivateStatic(
                patch.program.mixerGroup,
                clip,
                p,
                src,
                delay,
                volume * (1f + gain),
                1f
            );
            return looping;
        }

        internal static bool Activate(
            AudioProgram a,
            ActivationParams ap
#if UNITY_EDITOR
            ,
            Patch patch
#endif
        )
        {
            var p = a.audioParameters;
            float gain;
            var clip = a.GetClip(out gain);
#if UNITY_EDITOR
            if (!clip)
            {
                AppaLog.Error("Activate: Null AudioClip from patch: " + patch, patch);
            }
#endif
            ap.volume *= 1f + gain;
            var looping = Activate(
                a.mixerGroup,
                clip,
                p,
                ap
#if UNITY_EDITOR
                ,
                patch
#endif
            );
            return looping;
        }

        internal static bool Activate(
            AudioProgram a,
            AudioParameters.EnvelopeParams ep,
            ActivationParams ap
#if UNITY_EDITOR
            ,
            Patch patch
#endif
        )
        {
            var p = a.audioParameters;
            p.envelope = ep;
            float gain;
            var clip = a.GetClip(out gain);
#if UNITY_EDITOR
            if (!clip)
            {
                AppaLog.Error("Activate: Null AudioClip from patch: " + patch, patch);
            }
#endif
            ap.volume *= 1f + gain;
            var looping = Activate(
                a.mixerGroup,
                clip,
                p,
                ap
#if UNITY_EDITOR
                ,
                patch
#endif
            );
            return looping;
        }

        internal static bool Activate(AudioSequence s, ActivationParams ap)
        {
            var looping = false;
            for (int i = 0, n = s.timing.Length; i < n; ++i)
            {
                var g = s.timing[i];
                if (Randomizer.zeroToOne <= g.randomization.chance)
                {
                    var ap2 = ap;
                    ap2.delay += g.GetDelay();
                    if (g.patch != null)
                    {
                        looping |= g.patch.Activate(ap2);
                    }
                    else
                    {
                        looping |= Activate(
                            s.patch.program,
                            ap2
#if UNITY_EDITOR
                            ,
                            s.patch
#endif
                        );
                    }
                }
            }

            return looping;
        }

        internal static bool ActivateInternal(
            AudioMixerGroup g,
            AudioClip c,
            AudioParameters p,
            ActivationParams ap,
            Vector3 pos2,
            float pitch
#if UNITY_EDITOR
            ,
            Patch patch
#endif
        )
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.EditorApplication.CallbackFunction f = null;
                var n = Time.realtimeSinceStartup;
                f = () =>
                {
                    if ((Time.realtimeSinceStartup - n) >= ap.delay)
                    {
                        UnityEditor.EditorApplication.update -= f;
                        PlayClip(c, p.loop);
                    }
                };
                UnityEditor.EditorApplication.update += f;
                return false;
            }
#endif

            var i = freeSources.Count > 0 ? freeSources.Pop() : CreateSource();
            i.transform.position = pos2;
            i.Enable(p);

            var z = new ActiveSource
            {
                handle = ap.handle,
                keyOn = ap.delay,
                keyOff = false,
#if UNITY_EDITOR
                patch = patch,
#endif
                info = i,
                target = (ap.transform == null) || ap.transform.gameObject.isStatic ? null : ap.transform,
                localPosition = ap.position,
                volume = p.GetVolume() * Mathf.Clamp01(ap.volume),
                modVolume = ap.modVolume,
                envelope = Envelope.instant
            };

#if SYNTHESIZER_PARANOIA
        AppaLog.Info(string.Format(
            Time.frameCount.ToString("X4") +
            " Synthesizer.ActivateInternal: {0} {1} ({2})",
            g, c.name, z.target);
#endif

            if (p.envelope.attack > Mathf.Epsilon)
            {
                z.envelope.SetAttack(p.envelope.attack);
            }

            if (p.envelope.release > Mathf.Epsilon)
            {
                z.envelope.SetRelease(p.envelope.release);
            }

            ActivateStatic(g, c, p, i.audioSource, ap.delay, z.GetVolume(), pitch);
            activeSources0.Add(z);
            return p.loop;
        }

        internal static uint GetNextHandle()
        {
            activeHandle = (activeHandle << 16) | ((activeHandle & 0xffff) + 1);
            return activeHandle;
        }

        internal static void SetModVolume(uint handle, float volume)
        {
            if (handle == 0)
            {
                AppaLog.Error("SetModVolume: bad handle");
                return;
            }

            using (var enumerator = activeSources0.GetEnumerator())
            {
                for (var x = enumerator; x.MoveNext();)
                {
                    var z = x.Current;
                    if (z.handle == handle)
                    {
                        z.modVolume = volume;
                    }

                    activeSources1.Add(z);
                }
            }

            Swap(ref activeSources0, ref activeSources1);
            activeSources1.Clear();
        }

        internal static void Update(float dt)
        {
            using (var enumerator = activeSources0.GetEnumerator())
            {
                for (var x = enumerator; x.MoveNext();)
                {
                    var z = x.Current;
                    bool playing;
                    if (z.Check(out playing))
                    {
                        if (playing)
                        {
                            z.UpdatePosition();
                            z.UpdateEnvelope(dt, ref playing);
                            if (playing)
                            {
                                z.UpdateVolume();
                                activeSources1.Add(z);
                            }
                            else
                            {
#if SYNTHESIZER_PARANOIA
                        AppaLog.Info(string.Format(
                            Time.frameCount.ToString("X4") +
                            " Synthesizer.Update: {0} ({1}) : stopped by envelope",
                            z.info.audioSource.clip.name, z.info.audioSource.name);
#endif
                                z.info.audioSource.Stop();
                            }
                        }

                        if (!playing)
                        {
#if SYNTHESIZER_PARANOIA
                    AppaLog.Info(string.Format(
                        Time.frameCount.ToString("X4") +
                        " Synthesizer.Update: {0} ({1}) : freed",
                        z.info.audioSource.clip.name, z.info.audioSource.name);
#endif
                            z.info.Disable();
                            freeSources.Push(z.info);
                        }
                    }
                }
            }

            Swap(ref activeSources0, ref activeSources1);
            activeSources1.Clear();
        }

        private static bool ActivateStatic(
            AudioMixerGroup g,
            AudioClip c,
            AudioParameters p,
            AudioSource s,
            float delay,
            float volume,
            float pitch)
        {
            s.clip = c;
            s.volume = volume;
            s.pitch = p.GetPitch() * pitch * Time.timeScale;
            s.panStereo = p.panning;
            s.loop = p.loop;
            s.spatialBlend = p.spatial.blend;
            s.minDistance = p.spatial.distance.min;
            s.maxDistance = p.spatial.distance.max;
            s.dopplerLevel = p.spatial.doppler;
            s.priority = (int) p.runtime.priority;
            s.outputAudioMixerGroup = g;

            if (delay <= Mathf.Epsilon)
            {
                s.Play();
            }
            else
            {
                s.PlayDelayed(delay);
            }

#if SYNTHESIZER_PARANOIA
        AppaLog.Info(string.Format(
            Time.frameCount.ToString("X4") +
            " Synthesizer.ActivateStatic: {0} {1} ({2}) : {3:N2} {4:N2} {5:N2} -> {6}",
            g, c.name, s.name, delay, volume, pitch, s.isPlaying);
#endif

            return p.loop;
        }

        private static SourceInfo CreateSource()
        {
            var o = new GameObject(
#if UNITY_EDITOR
                $"Appalachia.Core.Audio.Source #{sourceIndex++:X2}"
#endif
            );
            o.transform.parent = Heartbeat.hierarchyTransform;
            var i = new SourceInfo
            {
                transform = o.transform,
                audioSource = o.AddComponent<AudioSource>(),
                lowPassFilter = o.AddComponent<AudioLowPassFilter>(),
                highPassFilter = o.AddComponent<AudioHighPassFilter>(),
                occlusion = o.AddComponent<Occlusion>()
            };
            i.audioSource.playOnAwake = false;
            return i;
        }

        private static void Swap<T>(ref List<T> a, ref List<T> b)
        {
            var y = a;
            a = b;
            b = y;
        }

        #region Nested Types

        [Serializable]
        public struct ActiveSource
        {
            #region Fields

            public bool keyOff;
            public Envelope envelope;
            public float keyOn;
            public float modVolume;
            public float volume;
#if UNITY_EDITOR
            public Patch patch;
#endif
            public SourceInfo info;
            public Transform target;
            public uint handle;
            public Vector3 localPosition;

            #endregion

            public bool Check(out bool playing)
            {
                if (info.audioSource)
                {
                    playing = info.audioSource.isPlaying || (info.audioSource.isVirtual && (keyOn > 0f));
                    return true;
                }

                playing = false;
                return false;
            }

            public float GetVolume()
            {
                var v = _masterVolume;
                v *= volume;
                v *= modVolume;
                v *= envelope.GetValue();
                return v;
            }

            public void UpdateEnvelope(float dt, ref bool playing)
            {
                if (keyOff)
                {
                    if (envelope.UpdateRelease(dt) >= 1f)
                    {
                        playing = false;
                    }
                }
                else if ((keyOn = Mathf.Max(0f, keyOn - dt)) <= 0f)
                {
                    envelope.UpdateAttack(dt);
                }
            }

            public void UpdatePosition()
            {
                if (target)
                {
                    info.transform.position = target.position + localPosition;
                }
            }

            public void UpdateVolume()
            {
                info.audioSource.volume = GetVolume();
            }
        }

        [Serializable]
        public struct SourceInfo
        {
            #region Fields

            public AudioHighPassFilter highPassFilter;
            public AudioLowPassFilter lowPassFilter;
            public AudioSource audioSource;
            public Occlusion occlusion;
            public Transform transform;

            #endregion

            public void Disable()
            {
                audioSource.enabled = false;
                lowPassFilter.enabled = false;
                highPassFilter.enabled = false;
                occlusion.enabled = false;
            }

            public void Enable(AudioParameters p)
            {
                audioSource.enabled = true;

                if ((p.occlusion.function != OcclusionFunction.None) && (p.spatial.blend > 0f))
                {
                    lowPassFilter.enabled = true;
                    highPassFilter.enabled = true;
                    occlusion.enabled = true;
                }

                occlusion.occlusion = p.occlusion;
                occlusion.spatial = p.spatial;
            }
        }

        #endregion

#if UNITY_EDITOR

        private static MethodInfo _PlayClip;
        private static MethodInfo _StopAllClips;

        private static void PlayClip(AudioClip c, bool loop)
        {
            if (_PlayClip == null)
            {
                var a = typeof(UnityEditor.EditorApplication).Assembly;
                var t = a.GetType("UnityEditor.AudioUtil");
                _PlayClip = t.GetMethod("PlayClip", new[] {typeof(AudioClip), typeof(int), typeof(bool)});
            }

            _PlayClip.Invoke(null, new object[] {c, 0, loop});
        }

        private static void StopAllClips()
        {
            if (_StopAllClips == null)
            {
                var a = typeof(UnityEditor.EditorApplication).Assembly;
                var t = a.GetType("UnityEditor.AudioUtil");
                _StopAllClips = t.GetMethod("StopAllClips", new Type[0]);
            }

            _StopAllClips.Invoke(null, new object[0]);
        }
#endif
    }
}
