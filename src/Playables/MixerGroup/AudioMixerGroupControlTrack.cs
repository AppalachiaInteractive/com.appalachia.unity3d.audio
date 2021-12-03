using UnityEngine.Audio;
using UnityEngine.Timeline;

namespace Appalachia.Audio.Playables.MixerGroup
{
    [TrackClipType(typeof(AudioMixerGroupClip))]
    [TrackBindingType(typeof(AudioMixerGroup))]
    public class AudioMixerGroupControlTrack : TrackAsset
    {
    }
}
