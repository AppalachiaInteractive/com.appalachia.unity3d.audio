using System;
using UnityEditor;
using UnityEngine;

namespace Appalachia.Audio
{
    public abstract class GUIHelpers
    {
        #region Static Fields and Autoproperties

        public static GUIStyle textStyle10 = BuildGUIStyleForLabel(
            Color.grey,
            10,
            false,
            FontStyle.Normal,
            TextAnchor.MiddleCenter
        );

        public static GUIStyle textStyle10RightAligned = BuildGUIStyleForLabel(
            Color.grey,
            10,
            false,
            FontStyle.Normal,
            TextAnchor.MiddleRight
        );

        #endregion

        public static GUIStyle BuildGUIStyleForLabel(
            Color color,
            int fontSize,
            bool wrapText,
            FontStyle fontstyle,
            TextAnchor anchor)
        {
            var style = new GUIStyle();
            style.focused.background = style.onNormal.background;
            style.focused.textColor = color;
            style.alignment = anchor;
            style.fontSize = fontSize;
            style.fontStyle = fontstyle;
            style.wordWrap = wrapText;
            style.clipping = TextClipping.Overflow;
            style.normal.textColor = color;
            return style;
        }

        public static void DrawDbTickMarks(
            Rect r,
            float yoffset,
            float yscale,
            Color textColor,
            Color lineColor)
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }

            textStyle10RightAligned.normal.textColor = textColor;
            float py = 10000.0f, h = 30, sy = 0.5f * r.height * yscale, cy = r.y + sy;
            for (float t = -200; t < 200; t += 1.0f)
            {
                var y = cy - (sy * (t - yoffset));
                if (((py - y) > h) && (y >= r.y) && (y <= (r.y + r.height + 10)))
                {
                    EditorGUI.DrawRect(new Rect(r.x, y, r.width, 1f), lineColor);

                    var text = $"{t:F0} dB";
                    GUI.Label(new Rect(r.x, y, 45f, 15f), text, textStyle10RightAligned);

                    py = y;
                }
            }
        }

        public static void DrawFrequencyTickMarks(Rect r, float samplerate, bool logScale, Color col)
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }

            textStyle10.normal.textColor = col;
            float px = r.x, w = 60.0f;
            for (float normFreq = 0; normFreq < 1.0f; normFreq += 0.01f)
            {
                var f = (float) MapNormalizedFrequency(normFreq, samplerate, logScale, true);
                var x = r.x + (normFreq * r.width);
                if ((x - px) > w)
                {
                    EditorGUI.DrawRect(new Rect(x, r.yMax - 5f, 1f, 5f), col);
                    GUI.Label(
                        new Rect(x, r.yMax - 22f, 1, 15f),
                        f < 1000.0f ? $"{f:F0} Hz" : $"{f * 0.001f:F1} kHz",
                        textStyle10
                    );
                    px = x;
                }
            }
        }

        public static void DrawLine(float x1, float y1, float x2, float y2, Color col)
        {
            Handles.color = col;
            Handles.DrawLine(new Vector3(x1, y1, 0), new Vector3(x2, y2, 0));
        }

        public static void DrawText(float x, float y, float w, string text, Color col)
        {
            textStyle10.normal.textColor = col;
            GUI.Label(new Rect(x, y, w, 10), text, textStyle10);
        }

        public static void DrawTimeTickMarks(Rect r, float window, Color textColor, Color lineColor)
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }

            textStyle10.normal.textColor = textColor;
            float px = 0.0f, w = 60;
            for (float t = 0; t < window; t += window * 0.01f)
            {
                var x = r.x + ((t * r.width) / window);
                if ((x - px) > w)
                {
                    var t0 = t - window;

                    EditorGUI.DrawRect(new Rect(x, r.y, 1f, r.height), lineColor);

                    var text = t0 > -1.0f ? $"{t0 * 1000.0f:F0} ms" : $"{t0:F1} s";
                    GUI.Label(new Rect(x, r.y, 1f, 15f), text, textStyle10);

                    px = x;
                }
            }
        }

        // Maps from normalized frequency to real frequency
        public static double MapNormalizedFrequency(double f, double sr, bool useLogScale, bool forward)
        {
            var maxFreq = 0.5 * sr;
            if (useLogScale)
            {
                const double lowestFreq = 10.0;
                if (forward)
                {
                    return lowestFreq * Math.Pow(maxFreq / lowestFreq, f);
                }

                return Math.Log(f / lowestFreq) / Math.Log(maxFreq / lowestFreq);
            }

            return forward ? f * maxFreq : f / maxFreq;
        }
    }
}
