using System;
using UnityEngine;

namespace Appalachia.Audio
{
    [Serializable]
    public class AudioImportSettingsOverrideSetting
    {
        public AudioImportTarget target = AudioImportTarget.Standalone;
        public AudioCompressionFormat compressionFormat = AudioCompressionFormat.ADPCM;
        public AudioClipLoadType loadType = AudioClipLoadType.CompressedInMemory;
        [Range(0f, 1f)] public float quality = 1f;
    }
}