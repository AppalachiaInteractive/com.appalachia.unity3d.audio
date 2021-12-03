using Appalachia.Audio.Behaviours;
using UnityEngine.Timeline;

namespace Appalachia.Audio.Playables.Emitter
{
    [TrackClipType(typeof(AudioEmitterClip))]
    [TrackBindingType(typeof(AudioEmitter))]
    public class AudioEmitterControlTrack : TrackAsset
    {
    }
}
