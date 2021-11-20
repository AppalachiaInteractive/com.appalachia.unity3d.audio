using System;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

namespace Appalachia.Audio
{
    public class ConvolutionReverbCustomGUI : IAudioEffectPluginGUI
    {
        #region Fields and Autoproperties

        private Color m_Impulse1Color = AudioCurveRendering.kAudioOrange;
        private Color m_Impulse2Color = AudioCurveRendering.kAudioOrange;

        #endregion

        public override string Description =>
            "Convolution reverb demo plugin for Unity's audio plugin system";

        public override string Name => "Demo ConvolutionReverb";

        public override string Vendor => "Unity";

        public override bool OnGUI(IAudioEffectPlugin plugin)
        {
            GUILayout.Space(5f);
            var r = GUILayoutUtility.GetRect(200, 150, GUILayout.ExpandWidth(true));
            DrawControl(plugin, r, plugin.GetSampleRate());
            GUILayout.Space(5f);
            return true;
        }

        public void DrawControl(IAudioEffectPlugin plugin, Rect r, float samplerate)
        {
            r = AudioCurveRendering.BeginCurveFrame(r);

            if (Event.current.type == EventType.Repaint)
            {
                var blend = plugin.IsPluginEditableAndEnabled() ? 1.0f : 0.5f;

                var numsamples = (int) r.width;
                float[] imp1;
                plugin.GetFloatBuffer("Impulse0", out imp1, numsamples);
                float[] imp2;
                plugin.GetFloatBuffer("Impulse1", out imp2, numsamples);

                float wet;
                plugin.GetFloatParameter("Wet", out wet);
                wet *= 0.01f;
                float gain;
                plugin.GetFloatParameter("Gain", out gain);
                gain = Mathf.Pow(10.0f, gain * 0.05f);
                float useSample;
                plugin.GetFloatParameter("Use Sample", out useSample);

                m_Impulse1Color.a = m_Impulse2Color.a = blend;

                var r2 = new Rect(r.x, r.y, r.width, r.height * 0.5f);
                DrawCurve(r2, imp1, 1.0f, m_Impulse1Color, 90, wet, gain);
                r2.y += r2.height;
                DrawCurve(r2, imp2, 1.0f, m_Impulse2Color, 150, wet, gain);

                var name = "Impulse: " +
                           Marshal.PtrToStringAnsi(ConvolutionReverb_GetSampleName((int) useSample));
                GUIHelpers.DrawText(r2.x + 5, r2.y - 5, r2.width, name, Color.white);
            }

            AudioCurveRendering.EndCurveFrame();
        }

        [DllImport("AudioPluginDemo")]
        private static extern IntPtr ConvolutionReverb_GetSampleName(int index);

        private void DrawCurve(
            Rect r,
            float[] curve,
            float yscale,
            Color col,

            // ReSharper disable once UnusedParameter.Local
            float labeloffset,
            float wetLevel,
            float gain)
        {
            float xscale = curve.Length - 2;
            var maxval = 0.0f;
            for (var n = 0; n < curve.Length; n++)
            {
                maxval = Mathf.Max(maxval, Mathf.Abs(curve[n]));
            }

            yscale *= maxval > 0.0f ? gain / maxval : 0.0f;
            AudioCurveRendering.DrawSymmetricFilledCurve(
                r,
                delegate(float x, out Color color)
                {
                    var f = Mathf.Clamp(x * xscale, 0.0f, xscale);
                    var i = (int) Mathf.Floor(f);
                    color = new Color(col.r, col.g, col.b, col.a * wetLevel);
                    return (curve[i] + ((curve[i + 1] - curve[i]) * (f - i))) * yscale;
                }
            );
        }
    }
}
