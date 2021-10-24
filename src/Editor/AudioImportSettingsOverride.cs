using System;

namespace Appalachia.Audio
{
    [Serializable]
    public class AudioImportSettingsOverride
    {
        public AudioImportSettingsOverrideSetting[] settings;
        public bool visible;
        public string filter;
    }
}
