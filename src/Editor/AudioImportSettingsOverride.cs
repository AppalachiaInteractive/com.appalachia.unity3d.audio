using System;

namespace Appalachia.Audio
{
    [Serializable]
    public class AudioImportSettingsOverride
    {
        public bool visible;
        public string filter;
        public AudioImportSettingsOverrideSetting[] settings;
    }
}