using System;

namespace Appalachia.Audio
{
    [Serializable]
    public class AudioImportSettingsOverride
    {
        #region Fields and Autoproperties

        public AudioImportSettingsOverrideSetting[] settings;
        public bool visible;
        public string filter;

        #endregion
    }
}
