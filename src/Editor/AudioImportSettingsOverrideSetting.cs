using System;
using Appalachia.Core.Objects.Root;
using UnityEngine;

namespace Appalachia.Audio
{
    [Serializable]
    public class AudioImportSettingsOverrideSetting : AppalachiaSimpleBase
    {
        #region Fields and Autoproperties

        public AudioClipLoadType loadType = AudioClipLoadType.CompressedInMemory;
        public AudioCompressionFormat compressionFormat = AudioCompressionFormat.ADPCM;
        public AudioImportTarget target = AudioImportTarget.Standalone;
        [Range(0f, 1f)] public float quality = 1f;

        #endregion
    }
}
