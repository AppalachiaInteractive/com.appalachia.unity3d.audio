using System.Collections.Generic;
using Appalachia.Audio.Utilities;
using UnityEditor;
using UnityEngine;

namespace Appalachia.Audio
{
    [CustomPropertyDrawer(typeof(ColorizeAttribute))]
    public class ColorizeDrawer : PropertyDrawer
    {
        private static readonly Dictionary<object, int> _lookup = new();

        private static readonly Color _disabledColor = new(0.75f, 0.75f, 0.75f);

        private static readonly Color[] _colors =
        {
            new(0.85f, 1.00f, 1.00f),
            new(0.85f, 1.00f, 0.85f),
            new(0.95f, 1.00f, 0.75f),
            new(1.00f, 0.75f, 0.65f),
            new(1.00f, 0.75f, 0.95f),
            new(0.75f, 0.75f, 1.00f),
            new(0.75f, 0.85f, 1.00f)
        };

        private static int _index;

        public static Color disabledColor => _disabledColor;

        public static void Reset()
        {
            _lookup.Clear();
            _index = 0;
        }

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

        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(prop, label, true);
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
