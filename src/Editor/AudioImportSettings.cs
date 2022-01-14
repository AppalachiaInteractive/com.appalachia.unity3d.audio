using Appalachia.Core.Objects.Root;

namespace Appalachia.Audio
{
    public class AudioImportSettings : SingletonAppalachiaObject<AudioImportSettings>
    {
        #region Fields and Autoproperties

        public AudioImportSettingsOverride[] overrides;

        #endregion
    }
}
