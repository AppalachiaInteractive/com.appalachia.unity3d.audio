using Appalachia.Audio.Utilities;
using Appalachia.Core.Objects.Root;

namespace Appalachia.Audio
{
    public class AudioImportSettings : SingletonAppalachiaObject<AudioImportSettings>
    {
         
        #region Fields and Autoproperties

        [Colorize] public AudioImportSettingsOverride[] overrides;

        #endregion
    }
}
