using UnityEditor;
using UnityEngine;

namespace Appalachia.Audio
{
    public class OscilloscopeCustomGUI : IAudioEffectPluginGUI
    {
        #region Constants and Static Readonly

        private const int maxspeclen = 4096;
        private const int scopeheight = 120;

        #endregion

        #region Fields and Autoproperties

        private readonly Color[] spec = new Color[maxspeclen];
        private readonly int[] specpos = new int[8];
        private readonly Texture2D[] spectex = new Texture2D[8];

        #endregion

        public override string Description => "Oscilloscope demo plugin for Unity's audio plugin system";

        public override string Name => "Demo Oscilloscope";

        public override string Vendor => "Unity";

        public override bool OnGUI(IAudioEffectPlugin plugin)
        {
            float active, window, scale, mode;
            plugin.GetFloatParameter("Active", out active);
            plugin.GetFloatParameter("Window", out window);
            plugin.GetFloatParameter("Scale",  out scale);
            plugin.GetFloatParameter("Mode",   out mode);
            GUILayout.Space(5.0f);

            DrawControl(
                plugin,
                GUILayoutUtility.GetRect(200, scopeheight, GUILayout.ExpandWidth(true)),
                plugin.GetSampleRate(),
                0
            );
            GUILayout.Space(5.0f);

            DrawControl(
                plugin,
                GUILayoutUtility.GetRect(200, scopeheight, GUILayout.ExpandWidth(true)),
                plugin.GetSampleRate(),
                1
            );
            GUILayout.Space(5.0f);

            return true;
        }

        public bool DrawControl(IAudioEffectPlugin plugin, Rect r, float samplerate, int channel)
        {
            r = AudioCurveRendering.BeginCurveFrame(r);

            if (Event.current.type == EventType.Repaint)
            {
                var blend = plugin.IsPluginEditableAndEnabled() ? 1.0f : 0.5f;

                float window, scale, mode;
                plugin.GetFloatParameter("Window", out window);
                plugin.GetFloatParameter("Scale",  out scale);
                plugin.GetFloatParameter("Mode",   out mode);

                float[] buffer;
                var numsamples = mode >= 1.0f ? maxspeclen : (int) (window * samplerate);
                plugin.GetFloatBuffer("Channel" + channel, out buffer, numsamples);
                numsamples = buffer.Length;

                if (mode < 2.0f)
                {
                    var lineColor = new Color(1.0f, 0.5f, 0.2f, blend);
                    if (mode >= 1.0f)
                    {
                        scale *= 0.1f;
                        AudioCurveRendering.DrawFilledCurve(
                            r,
                            delegate(float x)
                            {
                                var f = Mathf.Clamp(x * (numsamples - 2) * window * 0.5f, 0, numsamples - 2);
                                var i = (int) Mathf.Floor(f);
                                var s1 = 20.0f * Mathf.Log10(buffer[i] + 0.0001f);
                                var s2 = 20.0f * Mathf.Log10(buffer[i + 1] + 0.0001f);
                                return (s1 + ((s2 - s1) * (f - i))) * scale;
                            },
                            lineColor
                        );
                        GUIHelpers.DrawFrequencyTickMarks(r, samplerate * window * 0.5f, false, Color.red);
                        GUIHelpers.DrawDbTickMarks(
                            r,
                            1.0f / scale,
                            scale,
                            Color.red,
                            new Color(1.0f, 0.0f, 0.0f, 0.25f)
                        );
                    }
                    else
                    {
                        AudioCurveRendering.DrawCurve(
                            r,
                            delegate(float x)
                            {
                                return scale * buffer[(int) Mathf.Floor(x * (numsamples - 2))];
                            },
                            lineColor
                        );
                        GUIHelpers.DrawTimeTickMarks(
                            r,
                            window,
                            Color.red,
                            new Color(1.0f, 0.0f, 0.0f, 0.25f)
                        );
                    }
                }
                else
                {
                    scale *= 0.1f;

                    for (var i = 0; i < maxspeclen; i++)
                    {
                        var v = 20.0f * Mathf.Log10(buffer[i] + 0.0001f) * scale;
                        spec[i] = new Color(
                            Mathf.Clamp((v * 4.0f) - 1.0f, 0.0f, 1.0f),
                            Mathf.Clamp((v * 4.0f) - 2.0f, 0.0f, 1.0f),
                            1.0f -
                            (Mathf.Clamp(Mathf.Abs((v * 4.0f) - 1.0f), 0.0f, 1.0f) *
                             Mathf.Clamp(4.0f - (4.0f * v),            0.0f, 1.0f)),
                            1.0f
                        );
                    }

                    if (spectex[channel] == null)
                    {
                        spectex[channel] = new Texture2D(maxspeclen, scopeheight);
                    }

                    specpos[channel] = (specpos[channel] + 1) % scopeheight;
                    spectex[channel].SetPixels(0, specpos[channel], maxspeclen, 1, spec);
                    spectex[channel].Apply();

                    var oldColor = GUI.color;
                    GUI.color = new Color(1.0f, 1.0f, 1.0f, blend);

                    var r2 = new Rect(r.x, r.y + specpos[channel], r.width / (window * 0.5f), scopeheight);
                    GUI.DrawTexture(r2, spectex[channel], ScaleMode.StretchToFill, false, 1.0f);

                    r2.y -= scopeheight;
                    GUI.DrawTexture(r2, spectex[channel], ScaleMode.StretchToFill, false, 1.0f);

                    GUI.color = oldColor;

                    GUIHelpers.DrawFrequencyTickMarks(r, samplerate * window * 0.5f, false, Color.red);
                }
            }

            AudioCurveRendering.EndCurveFrame();
            return false;
        }
    }
}
