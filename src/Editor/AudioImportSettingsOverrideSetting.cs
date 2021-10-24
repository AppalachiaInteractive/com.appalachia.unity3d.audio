using System;
using UnityEngine;

namespace Appalachia.Audio
{
    [Serializable]
    public class AudioImportSettingsOverrideSetting
    {
        public AudioClipLoadType loadType = AudioClipLoadType.CompressedInMemory;
        public AudioCompressionFormat compressionFormat = AudioCompressionFormat.ADPCM;
        public AudioImportTarget target = AudioImportTarget.Standalone;
        [Range(0f, 1f)] public float quality = 1f;
    }
}
