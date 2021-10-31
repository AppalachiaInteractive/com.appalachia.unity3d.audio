using Appalachia.Audio.Utilities;
using Appalachia.Core.Scriptables;

namespace Appalachia.Audio
{
    public class AudioImportSettings : SingletonAppalachiaObject<AudioImportSettings>
    {
        [Colorize] public AudioImportSettingsOverride[] overrides;
    }
}
