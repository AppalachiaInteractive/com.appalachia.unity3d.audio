using Internal.Core.Audio;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

public class ChannelMonitorCustomGUI : IAudioEffectPluginGUI
{
    public override string Name => "Internal ChannelMonitor";

    public override string Description => "Channel monitor plugin for Unity's audio plugin system";

    public override string Vendor => "Unity";

    //private bool integratedFoldout = true;

    public AudioSignalSmoothAnalyzer loudness;
    private PropertyTree _loudnessTree;

    public void DrawControl(IAudioEffectPlugin plugin)
    {
        if (loudness == null)
        {
            loudness = new AudioSignalSmoothAnalyzer();
        }

        if (_loudnessTree == null)
        {
            _loudnessTree = PropertyTree.Create(loudness);
        }
        
        plugin.GetFloatParameter("Instance", out var instance);

        var i = GAC.ChannelMonitor_GetLoudnessData_dB((int)instance);

        loudness.Add(GAC.dBToNormalized(i));

        var guie = GUI.enabled;
        GUI.enabled = true;
        _loudnessTree.Draw(false);
        _loudnessTree.ApplyChanges();
        GUI.enabled = guie;


    }

    public override bool OnGUI(IAudioEffectPlugin plugin)
    {
        GUILayout.Space(5f);
        DrawControl(plugin);
        GUILayout.Space(5f);
        return true;
    }
}
