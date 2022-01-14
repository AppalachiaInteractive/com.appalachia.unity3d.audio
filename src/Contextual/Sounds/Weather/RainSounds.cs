using System;
using Appalachia.Audio.Contextual.Context;
using Appalachia.Audio.Contextual.Context.Collections;

namespace Appalachia.Audio.Contextual.Sounds.Weather
{
    [Serializable]
    public class RainSounds : AudioContextCollection2<Exposure_AudioContexts, RainStrength_AudioContexts,
        RainSounds>
    {
    }
}
