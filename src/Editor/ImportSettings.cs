using System;
using Appalachia.Audio.Utilities;
using UnityEditor;
using UnityEngine;

namespace Appalachia.Audio
{
    namespace Editor
    {
        public enum ImportTarget
        {
            Standalone,
            PS4,
            PSP2,
            XBoxOne,
            WSA,
            iOS,
            Android,
            WebGL
        }

        public class ImportSettings : ScriptableObject
        {
            public static readonly string path =
                "Assets/Features/Appalachia.Core.Audio/Editor/ImportSettings.asset";

            public string root = "Assets/Audio";

            [Colorize] public Override[] overrides;

            public static ImportSettings instance
            {
                get
                {
                    var s = AssetDatabase.LoadAssetAtPath<ImportSettings>(path);
                    if (s == null)
                    {
                        s = CreateInstance<ImportSettings>();
                        AssetDatabase.CreateAsset(s, path);
                    }

                    return s;
                }
            }

            [MenuItem("Appalachia.Core.Audio/Settings/Import Settings")]
            private static void PingImportSettings()
            {
                EditorGUIUtility.PingObject(instance);
            }

            [Serializable]
            public class Settings
            {
                public ImportTarget target = ImportTarget.Standalone;
                public AudioCompressionFormat compressionFormat = AudioCompressionFormat.ADPCM;
                public AudioClipLoadType loadType = AudioClipLoadType.CompressedInMemory;
                [Range(0f, 1f)] public float quality = 1f;
            }

            [Serializable]
            public class Override
            {
                public bool visible;
                public string filter;
                public Settings[] settings;
            }
        }
    } // Editor
}     // Appalachia.Core.Audio
