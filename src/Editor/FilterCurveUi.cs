using System;
using UnityEditor;
using UnityEngine;

namespace Appalachia.Audio
{
    public abstract class FilterCurveUi : IAudioEffectPluginGUI
    {
        #region Fields and Autoproperties

        protected DragOperation dragOperation = DragOperation.Low;

        #endregion

        public void DrawBandSplitMarker(
            IAudioEffectPlugin plugin,
            Rect r,
            float x,
            float w,
            bool highlight,
            Color color)
        {
            if (highlight)
            {
                w *= 2.0f;
            }

            EditorGUI.DrawRect(new Rect((r.x + x) - w, r.y, 2 * w, r.height), color);
        }

        public void DrawSpectrum(
            Rect r,
            bool useLogScale,
            float[] data,
            float dBRange,
            float samplerate,
            float colR,
            float colG,
            float colB,
            float colA,
            float gainOffsetDB)
        {
            var xscale = ((data.Length - 2) * 2.0f) / samplerate;
            var yscale = 1.0f / dBRange;
            AudioCurveRendering.DrawCurve(
                r,
                delegate(float x)
                {
                    var f = GUIHelpers.MapNormalizedFrequency(x, samplerate, useLogScale, true) * xscale;
                    var i = (int) Math.Floor(f);
                    var h = data[i] + ((data[i + 1] - data[i]) * (f - i));
                    var mag = h > 0.0 ? (20.0f * Math.Log10(h)) + gainOffsetDB : -120.0;
                    return (float) (yscale * mag);
                },
                new Color(colR, colG, colB, colA)
            );
        }

        protected static Color ScaleAlpha(Color col, float blend)
        {
            return new(col.r, col.g, col.b, col.a * blend);
        }

        #region Nested type: DragOperation

        protected enum DragOperation
        {
            Low,
            Mid,
            High
        }

        #endregion
    }
}
