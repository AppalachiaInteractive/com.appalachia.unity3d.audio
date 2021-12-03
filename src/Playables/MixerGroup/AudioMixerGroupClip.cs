using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Appalachia.Audio.Playables.MixerGroup
{//BokBok_Attentuation_Volume
    public class AudioMixerGroupClip : PlayableAsset, ITimelineClipAsset
    {
        #region Fields and Autoproperties

        public AudioMixerGroupControlBehaviour template;

        #endregion

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<AudioMixerGroupControlBehaviour>.Create(graph, template);
            return playable;
        }

        #region ITimelineClipAsset Members

        public ClipCaps clipCaps => ClipCaps.None;

        #endregion
    }
}
