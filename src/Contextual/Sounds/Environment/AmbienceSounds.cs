using System;
using Appalachia.Core.AssetMetadata.AudioMetadata.Context;
using Appalachia.Core.AssetMetadata.AudioMetadata.Context.Collections;

namespace Appalachia.Core.AssetMetadata.AudioMetadata.Sounds.Environment
{
    [Serializable] public class AmbienceSounds : AudioContextCollection4<Biome_AudioContexts, Season_AudioContexts, Weather_AudioContexts, TimeOfDay_AudioContexts,
        AmbienceSounds>
    {

    }
}