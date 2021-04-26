using System;
using MathHelpers;
using UnityEditor;
using UnityEngine;

public abstract class FilterCurveUi : IAudioEffectPluginGUI
{
    protected DragOperation dragOperation = DragOperation.Low;

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

    protected static Color ScaleAlpha(Color col, float blend)
    {
        return new Color(col.r, col.g, col.b, col.a * blend);
    }

    protected enum DragOperation
    {
        Low,
        Mid,
        High
    }
}

public class EqualizerCustomGUI : FilterCurveUi
{
    private float _masterGain;
    private float _lowGain, _midGain, _highGain;
    private float _lowFreq, _midFreq, _highFreq;
    private float _midQ, _lowQ, _highQ;
    private bool _useLogScale;
    private bool _showSpectrum;

    public override string Name => "Demo Equalizer";

    public override string Description => "3-band equalizer demo plugin for Unity's audio plugin system";

    public override string Vendor => "Unity";

    public void DrawFilterCurve(
        Rect r,
        float[] coeffs,
        bool lowGain,
        bool midGain,
        bool highGain,
        Color color,
        bool useLogScale,
        bool filled,
        double masterGain,
        double samplerate,
        double magScale)
    {
        var wm = (-2.0f * 3.1415926) / samplerate;

        var one = new ComplexD(1.0f, 0.0f);
        AudioCurveRendering.AudioCurveEvaluator d = delegate(float x)
        {
            var w = ComplexD.Exp(wm * GUIHelpers.MapNormalizedFrequency(x, samplerate, useLogScale, true));
            var hl = !lowGain
                ? one
                : ((w * ((w * coeffs[0]) + coeffs[1])) + coeffs[2]) / ((w * ((w * coeffs[3]) + coeffs[4])) + 1.0f);
            var hp = !midGain
                ? one
                : ((w * ((w * coeffs[5]) + coeffs[6])) + coeffs[7]) / ((w * ((w * coeffs[8]) + coeffs[9])) + 1.0f);
            var hh = !highGain
                ? one
                : ((w * ((w * coeffs[10]) + coeffs[11])) + coeffs[12]) / ((w * ((w * coeffs[13]) + coeffs[14])) + 1.0f);
            var h = hh * hp * hl;
            var mag = masterGain + (10.0 * Math.Log10(h.Mag2()));
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

        var thr = 4.0f;
        var changed = false;
        var x = evt.mousePosition.x - r.x;
        if ((evtType == EventType.MouseDown) && r.Contains(evt.mousePosition) && (evt.button == 0))
        {
            var lf = (float) GUIHelpers.MapNormalizedFrequency(_lowFreq, samplerate, _useLogScale, false) * r.width;
            var mf = (float) GUIHelpers.MapNormalizedFrequency(_midFreq, samplerate, _useLogScale, false) * r.width;
            var hf = (float) GUIHelpers.MapNormalizedFrequency(_highFreq, samplerate, _useLogScale, false) * r.width;
            var ld = Mathf.Abs(x - lf);
            var md = Mathf.Abs(x - mf);
            var hd = Mathf.Abs(x - hf);
            var d = ld;
            dragOperation = DragOperation.Low;
            if (md < d)
            {
                d = md;
                dragOperation = DragOperation.Mid;
            }

            if (hd < d)
            {
                d = hd;
                dragOperation = DragOperation.High;
            }

            GUIUtility.hotControl = controlID;
            EditorGUIUtility.SetWantsMouseJumping(1);
            evt.Use();
        }
        else if ((evtType == EventType.MouseDrag) && (GUIUtility.hotControl == controlID))
        {
            switch (dragOperation)
            {
                case DragOperation.Low:
                    _lowFreq = Mathf.Clamp(
                        (float) GUIHelpers.MapNormalizedFrequency(
                            GUIHelpers.MapNormalizedFrequency(_lowFreq, samplerate, _useLogScale, false) +
                            (evt.delta.x / r.width),
                            samplerate,
                            _useLogScale,
                            true
                        ),
                        10.0f,
                        samplerate * 0.5f
                    );
                    if (evt.shift)
                    {
                        _lowQ = Mathf.Clamp(_lowQ - (evt.delta.y * 0.05f), 0.01f, 10.0f);
                    }
                    else
                    {
                        _lowGain = Mathf.Clamp(_lowGain - (evt.delta.y * 0.5f), -100.0f, 100.0f);
                    }

                    break;
                case DragOperation.Mid:
                    _midFreq = Mathf.Clamp(
                        (float) GUIHelpers.MapNormalizedFrequency(
                            GUIHelpers.MapNormalizedFrequency(_midFreq, samplerate, _useLogScale, false) +
                            (evt.delta.x / r.width),
                            samplerate,
                            _useLogScale,
                            true
                        ),
                        10.0f,
                        samplerate * 0.5f
                    );
                    if (evt.shift)
                    {
                        _midQ = Mathf.Clamp(_midQ - (evt.delta.y * 0.05f), 0.01f, 10.0f);
                    }
                    else
                    {
                        _midGain = Mathf.Clamp(_midGain - (evt.delta.y * 0.5f), -100.0f, 100.0f);
                    }

                    break;
                case DragOperation.High:
                    _highFreq = Mathf.Clamp(
                        (float) GUIHelpers.MapNormalizedFrequency(
                            GUIHelpers.MapNormalizedFrequency(_highFreq, samplerate, _useLogScale, false) +
                            (evt.delta.x / r.width),
                            samplerate,
                            _useLogScale,
                            true
                        ),
                        10.0f,
                        samplerate * 0.5f
                    );
                    if (evt.shift)
                    {
                        _highQ = Mathf.Clamp(_highQ - (evt.delta.y * 0.05f), 0.01f, 10.0f);
                    }
                    else
                    {
                        _highGain = Mathf.Clamp(_highGain - (evt.delta.y * 0.5f), -100.0f, 100.0f);
                    }

                    break;
            }

            changed = true;
            evt.Use();
        }
        else if ((evtType == EventType.MouseUp) && (evt.button == 0) && (GUIUtility.hotControl == controlID))
        {
            GUIUtility.hotControl = 0;
            EditorGUIUtility.SetWantsMouseJumping(0);
            evt.Use();
        }

        if (Event.current.type == EventType.Repaint)
        {
            var blend = plugin.IsPluginEditableAndEnabled() ? 1.0f : 0.5f;

            // Mark bands (low, medium and high bands)
            DrawBandSplitMarker(
                plugin,
                r,
                (float) GUIHelpers.MapNormalizedFrequency(_lowFreq, samplerate, _useLogScale, false) * r.width,
                thr,
                (GUIUtility.hotControl == controlID) && (dragOperation == DragOperation.Low),
                new Color(0, 0, 0, blend)
            );
            DrawBandSplitMarker(
                plugin,
                r,
                (float) GUIHelpers.MapNormalizedFrequency(_midFreq, samplerate, _useLogScale, false) * r.width,
                thr,
                (GUIUtility.hotControl == controlID) && (dragOperation == DragOperation.Mid),
                new Color(0.5f, 0.5f, 0.5f, blend)
            );
            DrawBandSplitMarker(
                plugin,
                r,
                (float) GUIHelpers.MapNormalizedFrequency(_highFreq, samplerate, _useLogScale, false) * r.width,
                thr,
                (GUIUtility.hotControl == controlID) && (dragOperation == DragOperation.High),
                new Color(1.0f, 1.0f, 1.0f, blend)
            );

            const float dbRange = 40.0f;
            const float magScale = 1.0f / dbRange;

            float[] coeffs;
            plugin.GetFloatBuffer("Coeffs", out coeffs, 15);

            // Draw filled curve
            DrawFilterCurve(
                r,
                coeffs,
                true,
                true,
                true,
                ScaleAlpha(AudioCurveRendering.kAudioOrange, blend),
                _useLogScale,
                false,
                _masterGain,
                samplerate,
                magScale
            );

            if (GUIUtility.hotControl == controlID)
            {
                DrawFilterCurve(
                    r,
                    coeffs,
                    dragOperation == DragOperation.Low,
                    dragOperation == DragOperation.Mid,
                    dragOperation == DragOperation.High,
                    new Color(1.0f, 1.0f, 1.0f, 0.2f * blend),
                    _useLogScale,
                    true,
                    _masterGain,
                    samplerate,
                    magScale
                );
            }

            if (_showSpectrum)
            {
                var specLen = (int) r.width;
                float[] spec;

                plugin.GetFloatBuffer("InputSpec", out spec, specLen);
                DrawSpectrum(r, _useLogScale, spec, dbRange, samplerate, 0.3f, 1.0f, 0.3f, 0.5f * blend, 0.0f);

                plugin.GetFloatBuffer("OutputSpec", out spec, specLen);
                DrawSpectrum(r, _useLogScale, spec, dbRange, samplerate, 1.0f, 0.3f, 0.3f, 0.5f * blend, 0.0f);
            }

            GUIHelpers.DrawFrequencyTickMarks(r, samplerate, _useLogScale, new Color(1.0f, 1.0f, 1.0f, 0.3f * blend));
        }

        AudioCurveRendering.EndCurveFrame();
        return changed;
    }

    public override bool OnGUI(IAudioEffectPlugin plugin)
    {
        float useLogScaleFloat;
        float showSpectrumFloat;
        plugin.GetFloatParameter("MasterGain", out _masterGain);
        plugin.GetFloatParameter("LowGain", out _lowGain);
        plugin.GetFloatParameter("MidGain", out _midGain);
        plugin.GetFloatParameter("HighGain", out _highGain);
        plugin.GetFloatParameter("LowFreq", out _lowFreq);
        plugin.GetFloatParameter("MidFreq", out _midFreq);
        plugin.GetFloatParameter("HighFreq", out _highFreq);
        plugin.GetFloatParameter("LowQ", out _lowQ);
        plugin.GetFloatParameter("HighQ", out _highQ);
        plugin.GetFloatParameter("MidQ", out _midQ);
        plugin.GetFloatParameter("UseLogScale", out useLogScaleFloat);
        plugin.GetFloatParameter("ShowSpectrum", out showSpectrumFloat);
        _useLogScale = useLogScaleFloat > 0.5f;
        _showSpectrum = showSpectrumFloat > 0.5f;
        GUILayout.Space(5f);
        var r = GUILayoutUtility.GetRect(200, 100, GUILayout.ExpandWidth(true));
        if (DrawControl(plugin, r, plugin.GetSampleRate()))
        {
            plugin.SetFloatParameter("MasterGain", _masterGain);
            plugin.SetFloatParameter("LowGain", _lowGain);
            plugin.SetFloatParameter("MidGain", _midGain);
            plugin.SetFloatParameter("HighGain", _highGain);
            plugin.SetFloatParameter("LowFreq", _lowFreq);
            plugin.SetFloatParameter("HighFreq", _highFreq);
            plugin.SetFloatParameter("MidFreq", _midFreq);
            plugin.SetFloatParameter("LowQ", _lowQ);
            plugin.SetFloatParameter("MidQ", _midQ);
            plugin.SetFloatParameter("HighQ", _highQ);
        }

        return true;
    }
}
