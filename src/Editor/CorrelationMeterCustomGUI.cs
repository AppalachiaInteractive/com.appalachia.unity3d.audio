using UnityEditor;
using UnityEngine;

namespace Appalachia.Audio
{
    public class CorrelationMeterCustomGUI : IAudioEffectPluginGUI
    {
        #region Constants and Static Readonly

        private static readonly Vector3[] circle = new Vector3[40];
        private static readonly Vector3[] coord1 = new Vector3[2];
        private static readonly Vector3[] coord2 = new Vector3[2];

        #endregion

        /// <inheritdoc />
        public override string Description => "Correlation meter demo plugin for Unity's audio plugin system";

        /// <inheritdoc />
        public override string Name => "Demo CorrelationMeter";

        /// <inheritdoc />
        public override string Vendor => "Unity";

        /// <inheritdoc />
        public override bool OnGUI(IAudioEffectPlugin plugin)
        {
            // ReSharper disable once NotAccessedVariable
            float active, window, scale;
            plugin.GetFloatParameter("Active", out active);
            plugin.GetFloatParameter("Window", out window);
            plugin.GetFloatParameter("Scale",  out scale);
            GUILayout.Space(5.0f);
            var r = GUILayoutUtility.GetRect(200, 200, GUILayout.ExpandWidth(true));
            if (r.width > r.height)
            {
                r.width = r.height;
            }
            else
            {
                r.height = r.width;
            }

            if (DrawControl(plugin, r, plugin.GetSampleRate()))
            {
                plugin.SetFloatParameter("Window", window);
                plugin.SetFloatParameter("Scale",  scale);
            }

            GUILayout.Space(5.0f);
            return true;
        }

        public bool DrawControl(IAudioEffectPlugin plugin, Rect r, float samplerate)
        {
            r = AudioCurveRendering.BeginCurveFrame(r);

            if (Event.current.type == EventType.Repaint)
            {
                var blend = plugin.IsPluginEditableAndEnabled() ? 1.0f : 0.5f;

                float window;
                plugin.GetFloatParameter("Window", out window);
                window *= samplerate;
                if (window > samplerate)
                {
                    window = samplerate;
                }

                float[] corr;
                var numsamples = (int)window;
                plugin.GetFloatBuffer("Correlation", out corr, 2 * numsamples);
                numsamples = corr.Length;

                var cx = r.x + (r.width * 0.5f);
                var cy = r.y + (r.height * 0.5f);

                coord1[0].Set(r.x, r.y + (r.height * 0.5f), 0);
                coord1[1].Set(r.x + r.width, r.y + (r.height * 0.5f), 0);
                coord2[0].Set(r.x + (r.width * 0.5f), r.y, 0);
                coord2[1].Set(r.x + (r.width * 0.5f), r.y + r.height, 0);

                var w = (2.0f * 3.1415926f) / (circle.Length - 1);
                var cr = r.height * 0.4f;
                for (var n = 0; n < circle.Length; n++)
                {
                    circle[n].Set(cx + (cr * Mathf.Cos(n * w)), cy + (cr * Mathf.Sin(n * w)), 0);
                }

                float scale;
                plugin.GetFloatParameter("Scale", out scale);
                scale *= cr;

                var lineTint = 0.5f;
                Handles.color = new Color(lineTint, lineTint, lineTint, 0.75f);
                Handles.DrawAAPolyLine(2.0f, coord1.Length, coord1);
                Handles.DrawAAPolyLine(2.0f, coord2.Length, coord2);
                Handles.DrawAAPolyLine(2.0f, circle.Length, circle);

                HandleUtilityWrapper.handleWireMaterial.SetPass(0);
                GL.Begin(GL.LINES);
                var col1 = AudioCurveRendering.kAudioOrange;
                var col2 = Color.yellow;
                col1.a = blend;
                col2.a = 0.0f;
                var cs = 1.0f / ((numsamples / 2) - 1);
                for (var n = 0; n < (numsamples / 2); n++)
                {
                    var px = cx + (scale * corr[n * 2]);
                    var py = cy - (scale * corr[(n * 2) + 1]);
                    if ((px >= r.x) && (py >= r.y) && (px < (r.x + r.width)) && (py < (r.y + r.height)))
                    {
                        GL.Color(Color.Lerp(col1, col2, n * cs));
                        GL.Vertex3(px, py - 1.0f, 0.0f);
                        GL.Vertex3(px, py,        0.0f);
                    }
                }

                GL.End();
            }

            AudioCurveRendering.EndCurveFrame();
            return false;
        }
    }

// Missing API for UnityEditor.Handles.handleWireMaterial
}
