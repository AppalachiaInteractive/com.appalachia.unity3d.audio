using System;
using Appalachia.Core.Objects.Root;

namespace Appalachia.Audio
{
    [Serializable]
    public class AudioImportSettingsOverride : AppalachiaSimpleBase
    {
        #region Fields and Autoproperties

        public AudioImportSettingsOverrideSetting[] settings;
        public bool visible;
        public string filter;

        #endregion
    }
}
