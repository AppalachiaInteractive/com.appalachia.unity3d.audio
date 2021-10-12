using System;
using UnityEditor;

namespace Appalachia.Audio
{
    namespace Editor
    {
        public class AudioImporter : AssetPostprocessor
        {
            protected void OnPreprocessAudio()
            {
                int overrideIndex;

                if (!AudioImportSettingsEditor.overridesTable.TryGetValue(assetPath, out overrideIndex))
                {
                    return;
                }

                var importSettings = AudioImportSettings.instance;
                var @override = importSettings.overrides[overrideIndex];

                var importer = (UnityEditor.AudioImporter) assetImporter;

                var targets = Enum.GetNames(typeof(AudioImportTarget));
                var values = (int[]) Enum.GetValues(typeof(AudioImportTarget));

                for (int i = 0, n = targets.Length; i < n; ++i)
                {
                    importer.ClearSampleSettingOverride(targets[i]);

                    foreach (var settings in @override.settings)
                    {
                        if ((int) settings.target == values[i])
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
        }
    }
}     
