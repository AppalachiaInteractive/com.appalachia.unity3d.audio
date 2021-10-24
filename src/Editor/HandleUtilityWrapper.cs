using UnityEditor;
using UnityEngine;

namespace Appalachia.Audio
{
    public static class HandleUtilityWrapper
    {
        public static Material handleWireMaterial
        {
            get
            {
                if (s_Mat == null)
                {
                    s_Mat = (Material) EditorGUIUtility.LoadRequired("SceneView/HandleLines.mat");
                }

                return s_Mat;
            }
        }

        private static Material s_Mat;
    }
}
