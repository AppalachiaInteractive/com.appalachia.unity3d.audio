using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Appalachia.Audio.Playables.Emitter
{
    //BokBok_Attentuation_Volume
    public class AudioEmitterClip : PlayableAsset, ITimelineClipAsset
    {
        #region Fields and Autoproperties

        public AudioEmitterControlBehaviour template;

        #endregion

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<AudioEmitterControlBehaviour>.Create(graph, template);
            return playable;
        }

        #region ITimelineClipAsset Members

        public ClipCaps clipCaps => ClipCaps.None;

        #endregion
    }
}
