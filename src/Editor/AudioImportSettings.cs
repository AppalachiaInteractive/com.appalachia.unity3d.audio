using Appalachia.Audio.Utilities;
using Appalachia.Core.Scriptables;

namespace Appalachia.Audio
{
    public class AudioImportSettings : SelfSavingSingletonScriptableObject<AudioImportSettings>
    {
        [Colorize] public AudioImportSettingsOverride[] overrides;

    }
}