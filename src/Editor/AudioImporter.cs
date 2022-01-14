using System;
using Appalachia.Core.Attributes;
using UnityEditor;

namespace Appalachia.Audio
{
    [CallStaticConstructorInEditor]
    public class AudioImporter : AssetPostprocessor
    {
        static AudioImporter()
        {
            AudioImportSettings.InstanceAvailable += i => _audioImportSettings = i;
        }

        #region Static Fields and Autoproperties

        private static AudioImportSettings _audioImportSettings;

        #endregion

        #region Event Functions

        protected void OnPreprocessAudio()
        {
            if (!AudioImportSettings.IsInstanceAvailable)
            {
                return;
            }

            int overrideIndex;

            if (!AudioImportSettingsEditor.overridesTable.TryGetValue(assetPath, out overrideIndex))
            {
                return;
            }

            var @override = _audioImportSettings.overrides[overrideIndex];

            var importer = (UnityEditor.AudioImporter)assetImporter;

            var targets = Enum.GetNames(typeof(AudioImportTarget));
            var values = (int[])Enum.GetValues(typeof(AudioImportTarget));

            for (int i = 0, n = targets.Length; i < n; ++i)
            {
                importer.ClearSampleSettingOverride(targets[i]);

                foreach (var settings in @override.settings)
                {
                    if ((int)settings.target == values[i])
                    {
                        var sampleSettings = new AudioImporterSampleSettings
                        {
                            compressionFormat = settings.compressionFormat,
                            loadType = settings.loadType,
                            quality = settings.quality,
                            sampleRateOverride = 44100
                        };

                        if (settings.target == AudioImportTarget.Standalone)
                        {
                            importer.defaultSampleSettings = sampleSettings;
                        }
                        else
                        {
                            importer.SetOverrideSampleSettings(targets[i], sampleSettings);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
