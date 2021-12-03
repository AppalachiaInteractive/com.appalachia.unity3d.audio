#region

using System;
using Appalachia.Audio.Core;
using Appalachia.Audio.Utilities;
using Appalachia.Core.Scriptables;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Appalachia.Audio.Scriptables
{
    public sealed class Patch : AppalachiaObject
    {
        #region Fields and Autoproperties

        [BoxGroup("Program"), HideLabel] public AudioProgram program;

        [BoxGroup("Sequence"), HideLabel] public AudioSequence sequence;

        [NonSerialized] public bool hasTimings;

        #endregion

        #region Event Functions

        protected override void OnEnable()
        {
            base.OnEnable();
            if (program != null)
            {
                program.patch = this;
                program.Initialize();
            }

            if (sequence != null)
            {
                sequence.patch = this;
                hasTimings = (sequence.timing != null) && (sequence.timing.Length > 0);
            }
        }

        #endregion

        public bool Activate(ActivationParams ap)
        {
            return hasTimings
                ? sequence.Activate(ap)
                : program.Activate(
                    ap
#if UNITY_EDITOR
                    ,
                    this
#endif
                );
        }

        public bool Activate(ActivationParams ap, AudioParameters.EnvelopeParams ep)
        {
            return program.Activate(
                ap,
                ep
#if UNITY_EDITOR
                ,
                this
#endif
            );
        }

        public int GetClipIndex()
        {
            return !program.randomize ? program.clipIndex : -1;
        }

        public bool GetCueInfo(out float duration, out int repeats)
        {
            if (hasTimings)
            {
                return sequence.GetCueInfo(out duration, out repeats);
            }

            duration = 0f;
            repeats = 0;
            return false;
        }

        public void SetClipIndex(int index)
        {
            if (!program.randomize)
            {
                program.clipIndex = index;
            }
        }

        internal float GetMaxDuration()
        {
            return Mathf.Max(program.GetMaxDuration(), sequence.GetMaxDuration());
        }

#if UNITY_EDITOR

        #region Fields and Autoproperties

        private bool _foldout;
        private float _random;
        private float[] _weights;
        private string _played;

        #endregion

        [Button]
        private void SetClipsToSelected()
        {
            var assets = UnityEditor.Selection.GetFiltered(
                typeof(AudioClip),
                UnityEditor.SelectionMode.Assets
            );
            Array.Sort(assets, (x, y) => string.Compare(x.name, y.name));
            program.clips = new AudioProgram.AudioClipParams[assets.Length];

            for (int i = 0, n = assets.Length; i < n; ++i)
            {
                program.clips[i] = new AudioProgram.AudioClipParams { clip = (AudioClip)assets[i] };
            }
        }

        private void DrawAudioProgramInspectorGUI()
        {
            AudioClip c;

            GUILayout.BeginHorizontal();
            GUI.color = new Color(0.75f, 1.00f, 0.75f);
            if (GUILayout.Button("\u25b6"))
            {
                float gain;
                if (program.randomize)
                {
                    _random = Randomizer.zeroToOne;
                    program.weighted.count = program.clips.Length;
                    _weights = (float[])program.weighted.weights.Clone();
                    c = program.GetClip(_random, out gain);
                }
                else
                {
                    c = program.GetClip(out gain);
                }

                if (c != null)
                {
                    _played = c.name;
                    Synthesizer.KeyOn(null, c, program.audioParameters, null, new Vector3(), 1f + gain);
                }
            }

            GUI.color = new Color(1.00f, 0.75f, 0.75f);
            if (GUILayout.Button("\u2585"))
            {
                Synthesizer.StopAll();
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(16);

            GUI.color = Color.white;
            _foldout = UnityEditor.EditorGUILayout.Foldout(_foldout, "Randomization");
            if (_foldout && (_weights != null))
            {
                float s = 0;
                for (int i = 0, n = _weights.Length; i < n; ++i)
                {
                    s += _weights[i];
                }

                var t = _random * s;
                GUILayout.BeginHorizontal();
                GUILayout.Label(s.ToString("N2"));
                GUILayout.Label(t.ToString("N2"));
                GUILayout.Label("\t");
                GUILayout.EndHorizontal();
                for (int i = 0, n = _weights.Length; i < n; ++i)
                {
                    if (t >= _weights[i])
                    {
                        GUI.color = Color.white;
                    }
                    else if (program.clips[i].clip.name == _played)
                    {
                        GUI.color = Color.green;
                    }
                    else
                    {
                        GUI.color = Color.gray;
                    }

                    t -= _weights[i];
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(_weights[i].ToString("N2"));
                    GUILayout.Label(t.ToString("N2"));
                    GUILayout.Label(program.clips[i].clip.name);
                    GUILayout.EndHorizontal();
                }
            }
        }

        [ButtonGroup("Play-Stop")]
        [LabelText("\u25b6")]
        [GUIColor(0.75f, 1.00f, 0.75f)]
        private void PlayButton()
        {
            Synthesizer.KeyOn(out _, program.patch);
        }

        [ButtonGroup("Play-Stop")]
        [LabelText("\u2585")]
        [GUIColor(1.00f, 0.75f, 0.75f)]
        private void StopButton()
        {
            Synthesizer.StopAll();
        }
#endif
    }
}