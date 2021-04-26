#region

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

#endregion

namespace Internal.Core.Audio
{
    public class OcclusionSettings : ScriptableObject
    {
        public static readonly string path = "Assets/Features/Internal.Core.Audio/Resources/OcclusionSettings.asset";

        public LayerMask layerMask = 1;

        [MinMax(0, 22000)] public MinMaxFloat highPassRange = new MinMaxFloat {min = 0, max = 1100};

        [MinMax(0, 22000)] public MinMaxFloat lowPassRange = new MinMaxFloat {min = 4400, max = 22000};

        public float speedOfSound = 340f;

        public static OcclusionSettings instance
        {
            get
            {
                var s = Resources.Load<OcclusionSettings>("OcclusionSettings");
#if UNITY_EDITOR
                if (s == null)
                {
                    s = CreateInstance<OcclusionSettings>();
                    AssetDatabase.CreateAsset(s, path);
                }
#endif
                return s;
            }
        }

#if UNITY_EDITOR
        [MenuItem("Internal.Core.Audio/Settings/Occlusion Settings")]
        private static void PingImportSettingst()
        {
            EditorGUIUtility.PingObject(instance);
        }
#endif
    }
} // Internal.Core.Audio
