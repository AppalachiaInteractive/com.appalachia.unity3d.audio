using System;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

namespace Appalachia.Audio
{
    public class GranulatorCustomGUI : IAudioEffectPluginGUI
    {
        protected string[] DragParams = {"None", "Rnd offset", "Offset", "Window len", "Rnd window len"};

        protected DragOperation dragOperation = DragOperation.None;

        private Color m_Wave1Color = AudioCurveRendering.kAudioOrange;
        private Color m_Wave2Color = new Color(1.0f, 0.3f, 0.3f, 1.0f);
        private Color m_Wave3Color = new Color(0.7f, 0.3f, 0.3f, 1.0f);
        private Color m_Wave4Color = AudioCurveRendering.kAudioOrange;

        public override string Name => "Demo Granulator";

        public override string Description => "Granular synthesizer demo plugin for Unity's audio plugin system";

        public override string Vendor => "Unity";

        [DllImport("AudioPluginDemo")]
        private static extern IntPtr Granulator_GetSampleName(int index);

        private void DrawCurve(
            Rect r,
            float[] curve,
            float yscale,
            Color c0,
            Color c1,
            Color c2,
            Color c3,
            // ReSharper disable once UnusedParameter.Local
            float labeloffset,
            float rofs,
            float ofs,
            float wlen,
            float rwlen,
            float shape)
        {
            float xscale = curve.Length - 2;
            var maxval = 0.0f;
            for (var n = 0; n < curve.Length; n++)
            {
                maxval = Mathf.Max(maxval, Mathf.Abs(curve[n]));
            }

            yscale *= maxval > 0.0f ? 1.0f / maxval : 0.0f;
            float transition = (0.5f * (wlen + rwlen)) / shape, mix = 0.7f;
            var transitionScale = transition > 0.0f ? 1.0f / transition : 0.0f;
            AudioCurveRendering.DrawSymmetricFilledCurve(
                r,
                delegate(float x, out Color color)
                {
                    color = c0;
                    var f = Mathf.Clamp(x * xscale, 0.0f, xscale);
                    var i = (int) Mathf.Floor(f);
                    var y = (curve[i] + ((curve[i + 1] - curve[i]) * (f - i))) * yscale;
                    x -= ofs;
                    if (x >= -rofs)
                    {
                        if (x < 0.0f)
                        {
                            color = Color.Lerp(color, c3, mix);
                        }

                        if ((x + rofs) < transition)
                        {
                            y *= (x + rofs) * transitionScale;
                        }
                        else if ((x < (wlen + rwlen)) && (x > ((wlen + rwlen) - transition)))
                        {
                            y *= ((wlen + rwlen) - x) * transitionScale;
                        }

                        if (x < wlen)
                        {
                            color = Color.Lerp(color, c1, mix);
                        }
                        else if (x < (wlen + rwlen))
                        {
                            color = Color.Lerp(color, c2, mix);
                        }
                        else
                        {
                            color.a *= 0.4f;
                        }
                    }
                    else
                    {
                        color.a *= 0.4f;
                    }

                    return y;
                }
            );
        }

        public void DrawControl(IAudioEffectPlugin plugin, Rect r, float samplerate)
        {
            var evt = Event.current;
            var controlID = GUIUtility.GetControlID(FocusType.Passive);
            var evtType = evt.GetTypeForControl(controlID);

            r = AudioCurveRendering.BeginCurveFrame(r);

            if ((evtType == EventType.MouseDown) && r.Contains(evt.mousePosition) && (evt.button == 0))
            {
                float ofs, rofs;
                plugin.GetFloatParameter(DragParams[(int) DragOperation.Offset],       out ofs);
                plugin.GetFloatParameter(DragParams[(int) DragOperation.RandomOffset], out rofs);
                var x = r.x + (r.width * (ofs - rofs));
                var mindist = Mathf.Abs(x - evt.mousePosition.x);
                dragOperation = DragOperation.RandomOffset;
                x = r.x;
                for (var i = DragOperation.Offset; i <= DragOperation.RandomWindowLen; i++)
                {
                    float value;
                    plugin.GetFloatParameter(DragParams[(int) i], out value);
                    x += r.width * value;
                    var dx = Mathf.Abs(x - evt.mousePosition.x);
                    if (dx < mindist)
                    {
                        mindist = dx;
                        dragOperation = i;
                    }
                }

                GUIUtility.hotControl = controlID;
                EditorGUIUtility.SetWantsMouseJumping(1);
                evt.Use();
            }

            if ((evtType == EventType.MouseDrag) && (GUIUtility.hotControl == controlID))
            {
                if (dragOperation != DragOperation.None)
                {
                    float value;
                    plugin.GetFloatParameter(DragParams[(int) dragOperation], out value);
                    value = Mathf.Clamp(
                        value + ((dragOperation == DragOperation.RandomOffset ? -evt.delta.x : evt.delta.x) * 0.001f),
                        0.0f,
                        1.0f
                    );
                    plugin.SetFloatParameter(DragParams[(int) dragOperation], value);
                    evt.Use();
                }
            }
            else if ((evtType == EventType.MouseUp) && (GUIUtility.hotControl == controlID))
            {
                dragOperation = DragOperation.None;
                GUIUtility.hotControl = 0;
                EditorGUIUtility.SetWantsMouseJumping(0);
                evt.Use();
            }
            else if (Event.current.type == EventType.Repaint)
            {
                var blend = plugin.IsPluginEditableAndEnabled() ? 1.0f : 0.5f;

                var numsamples = (int) r.width;
                float[] wave1;
                plugin.GetFloatBuffer("Waveform0", out wave1, numsamples);
                float[] wave2;
                plugin.GetFloatBuffer("Waveform1", out wave2, numsamples);

                float ofs;
                plugin.GetFloatParameter(DragParams[(int) DragOperation.Offset], out ofs);
                float rofs;
                plugin.GetFloatParameter(DragParams[(int) DragOperation.RandomOffset], out rofs);
                float wlen;
                plugin.GetFloatParameter(DragParams[(int) DragOperation.WindowLen], out wlen);
                float rwlen;
                plugin.GetFloatParameter(DragParams[(int) DragOperation.RandomWindowLen], out rwlen);
                float shape;
                plugin.GetFloatParameter("Shape", out shape);
                float useSample;
                plugin.GetFloatParameter("Use Sample", out useSample);

                m_Wave1Color.a = blend;
                m_Wave2Color.a = blend;
                m_Wave3Color.a = blend;
                m_Wave4Color.a = blend;

                var r2 = new Rect(r.x, r.y, r.width, r.height * 0.5f);
                DrawCurve(
                    r2,
                    wave1,
                    1.0f,
                    m_Wave1Color,
                    m_Wave2Color,
                    m_Wave3Color,
                    m_Wave4Color,
                    90,
                    rofs,
                    ofs,
                    wlen,
                    rwlen,
                    shape
                );
                r2.y += r2.height;
                DrawCurve(
                    r2,
                    wave2,
                    1.0f,
                    m_Wave1Color,
                    m_Wave2Color,
                    m_Wave3Color,
                    m_Wave4Color,
                    150,
                    rofs,
                    ofs,
                    wlen,
                    rwlen,
                    shape
                );

                var x1 = r.x + (r.width * (ofs - rofs));
                var x2 = r.x + (r.width * ofs);
                var x3 = x2 + (r.width * wlen);
                var x4 = x3 + (r.width * rwlen);
                GUIHelpers.DrawLine(x1, r.y, x1, r.y + r.height, m_Wave1Color);
                GUIHelpers.DrawLine(x2, r.y, x2, r.y + r.height, m_Wave2Color);
                GUIHelpers.DrawLine(x3, r.y, x3, r.y + r.height, m_Wave3Color);
                GUIHelpers.DrawLine(x4, r.y, x4, r.y + r.height, m_Wave4Color);

                var name = "Sample: " + Marshal.PtrToStringAnsi(Granulator_GetSampleName((int) useSample));
                GUIHelpers.DrawText(r2.x + 5, r2.y - 5, r2.width, name, Color.white);
            }

            AudioCurveRendering.EndCurveFrame();
        }

        public override bool OnGUI(IAudioEffectPlugin plugin)
        {
            GUILayout.Space(5f);
            var r = GUILayoutUtility.GetRect(200, 150, GUILayout.ExpandWidth(true));
            DrawControl(plugin, r, plugin.GetSampleRate());
            GUILayout.Space(5f);
            return true;
        }

        protected enum DragOperation
        {
            None,
            RandomOffset,
            Offset,
            WindowLen,
            RandomWindowLen
        }
    }
}
