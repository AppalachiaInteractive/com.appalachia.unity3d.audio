using System.Collections.Generic;
using Appalachia.Audio.Utilities;
using UnityEditor;
using UnityEngine;

namespace Appalachia.Audio
{
    [CustomPropertyDrawer(typeof(ColorizeAttribute))]
    public class ColorizeDrawer : PropertyDrawer
    {
        #region Constants and Static Readonly

        private static readonly Color _disabledColor = new(0.75f, 0.75f, 0.75f);

        private static readonly Color[] _colors =
        {
            new(0.85f, 1.00f, 1.00f, 1.0f),
            new(0.85f, 1.00f, 0.85f, 1.0f),
            new(0.95f, 1.00f, 0.75f, 1.0f),
            new(1.00f, 0.75f, 0.65f, 1.0f),
            new(1.00f, 0.75f, 0.95f, 1.0f),
            new(0.75f, 0.75f, 1.00f, 1.0f),
            new(0.75f, 0.85f, 1.00f, 1.0f)
        };

        private static readonly Dictionary<object, int> _lookup = new();

        #endregion

        #region Static Fields and Autoproperties

        private static int _index;

        #endregion

        public static Color disabledColor => _disabledColor;

        public static Color GetColor(int index)
        {
            return _colors[index % _colors.Length];
        }

        public static Color GetColor(string path)
        {
            int i, j;
            while ((i = path.IndexOf('[')) > 0)
            {
                j = path.IndexOf(']');
                path = path.Substring(0, i) + path.Substring(j + 1);
            }

            if (!_lookup.TryGetValue(path, out i))
            {
                i = _index;
                _lookup[path] = i;
                _index = i + 1;
            }

            return GetColor(i);
        }

        public static void Reset()
        {
            _lookup.Clear();
            _index = 0;
        }

        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            var height = EditorGUI.GetPropertyHeight(prop, label, true);

            return height;
        }

        public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
        {
            var oldColor = GUI.color;
            GUI.color = GetColor(prop.propertyPath);
            EditorGUI.PropertyField(pos, prop, label, true);
            GUI.color = oldColor;
        }
    }
}
