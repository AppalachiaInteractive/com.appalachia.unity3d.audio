using System;
using UnityEditor;
using UnityEngine;

namespace Appalachia.Audio
{
    public class ModalFilterCustomGUI : FilterCurveUi
    {
        public override string Name => "Demo ModalFilter";

        public override string Description =>
            "Modal filter demo plugin for Unity's audio plugin system";

        public override string Vendor => "Unity";

        public void DrawFilterCurve(
            Rect r,
            float[] coeffs,
            Color color,
            int numModes,
            bool useLogScale,
            bool filled,
            double samplerate,
            double magScale)
        {
            var wm = (-2.0f * 3.1415926) / samplerate;

            var zero = new ComplexD(0.0f, 0.0f);
            var one = new ComplexD(1.0f,  0.0f);
            AudioCurveRendering.AudioCurveEvaluator d = delegate(float x)
            {
                var w = ComplexD.Exp(
                    wm * GUIHelpers.MapNormalizedFrequency(x, samplerate, useLogScale, true)
                );
                var h = zero;
                var num = numModes * 3;
                for (var n = 0; n < num; n += 3)
                {
                    h += (coeffs[n] * (one - (w * w))) /
                         ((w * ((w * coeffs[n + 2]) + coeffs[n + 1])) + 1.0);
                }

                var mag = 10.0 * Math.Log10(h.Mag2());
                return (float) (mag * magScale);
            };

            if (filled)
            {
                AudioCurveRendering.DrawFilledCurve(r, d, color);
            }
            else
            {
                AudioCurveRendering.DrawCurve(r, d, color);
            }
        }

        public bool DrawControl(IAudioEffectPlugin plugin, Rect r, float samplerate)
        {
            var evt = Event.current;
            var controlID = GUIUtility.GetControlID(FocusType.Passive);
            var evtType = evt.GetTypeForControl(controlID);

            r = AudioCurveRendering.BeginCurveFrame(r);

            if (Event.current.type == EventType.Repaint)
            {
                var blend = plugin.IsPluginEditableAndEnabled() ? 1.0f : 0.5f;

                const float dbRange = 40.0f;
                const float magScale = 1.0f / dbRange;

                float showSpectrum;
                plugin.GetFloatParameter("ShowSpectrum", out showSpectrum);
                if (showSpectrum >= 0.5f)
                {
                    blend *= 0.5f;
                }

                var useLogScale = false;

                float numModes = 0;
                if (plugin.GetFloatParameter("Num modes", out numModes) &&
                    (numModes > 0) &&
                    (numModes < 1000))
                {
                    float[] coeffs;
                    if (plugin.GetFloatBuffer("Coeffs", out coeffs, (int) numModes * 3) &&
                        (coeffs != null))
                    {
                        // Draw filled curve
                        DrawFilterCurve(
                            r,
                            coeffs,
                            ScaleAlpha(AudioCurveRendering.kAudioOrange, blend),
                            (int) numModes,
                            useLogScale,
                            false,
                            samplerate,
                            magScale
                        );

                        GUIHelpers.DrawFrequencyTickMarks(
                            r,
                            samplerate,
                            useLogScale,
                            new Color(1.0f, 1.0f, 1.0f, 0.3f * blend)
                        );
                    }
                }

                if (showSpectrum >= 0.5f)
                {
                    float spectrumOffset;
                    plugin.GetFloatParameter("SpectrumOffset", out spectrumOffset);

                    var specLen = (int) r.width;
                    float[] spec;

                    plugin.GetFloatBuffer("InputSpec", out spec, specLen);
                    DrawSpectrum(
                        r,
                        useLogScale,
                        spec,
                        dbRange,
                        samplerate,
                        0.3f,
                        1.0f,
                        0.3f,
                        0.5f * blend,
                        spectrumOffset
                    );

                    plugin.GetFloatBuffer("OutputSpec", out spec, specLen);
                    DrawSpectrum(
                        r,
                        useLogScale,
                        spec,
                        dbRange,
                        samplerate,
                        1.0f,
                        0.3f,
                        0.3f,
                        0.5f * blend,
                        spectrumOffset
                    );
                }
            }

            AudioCurveRendering.EndCurveFrame();
            return false;
        }

        public override bool OnGUI(IAudioEffectPlugin plugin)
        {
            GUILayout.Space(5f);
            var r = GUILayoutUtility.GetRect(200, 100, GUILayout.ExpandWidth(true));
            DrawControl(plugin, r, plugin.GetSampleRate());
            return true;
        }
    }
}
