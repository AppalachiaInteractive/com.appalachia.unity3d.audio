using System;
using Appalachia.Audio.Contextual.Context;
using Appalachia.Audio.Contextual.Context.Collections;

namespace Appalachia.Audio.Contextual.Sounds.Environment
{
    [Serializable]
    public class AmbienceSounds : AudioContextCollection4<BiomeAudioContext, SeasonAudioContext,
        WeatherAudioContext, TimeOfDayAudioContext, AmbienceSounds>
    {
    }
}
