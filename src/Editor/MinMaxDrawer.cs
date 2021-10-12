using Appalachia.Audio.Utilities;
using UnityEditor;
using UnityEngine;

namespace Appalachia.Audio
{
    namespace Editor
    {
        [CustomPropertyDrawer(typeof(MinMaxAttribute))]
        public class MinMaxDrawer : PropertyDrawer
        {
            public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
            {
                var mp = prop.FindPropertyRelative("min");
                var np = prop.FindPropertyRelative("max");
                var mm = attribute as MinMaxAttribute;

                if ((mp != null) && (np != null) && (mm != null))
                {
                    var oldColor = GUI.color;
                    if (mm.colorize)
                    {
                        GUI.color = ColorizeDrawer.GetColor(prop.propertyPath);
                    }

                    var i = EditorGUI.indentLevel;

                    var mv = mp.floatValue;
                    var nv = np.floatValue;

                    var dx1 = (EditorGUIUtility.fieldWidth * 2) + (Mathf.Clamp01(i) * 9);
                    var dx2 = (EditorGUIUtility.fieldWidth * 2) + ((i - 1) * 9);

                    var r = pos;
                    r.width = r.width - dx1;
                    EditorGUI.MinMaxSlider(
                        r,
                        new GUIContent(ObjectNames.NicifyVariableName(prop.name)),
                        ref mv,
                        ref nv,
                        mm.min,
                        mm.max
                    );

                    EditorGUI.indentLevel = 0;

                    r.x = (pos.width - dx2) + (i * 9) + 3;
                    r.width = EditorGUIUtility.fieldWidth;
                    var s = new GUIStyle(EditorStyles.numberField);
                    s.fixedWidth = EditorGUIUtility.fieldWidth;
                    mv = EditorGUI.DelayedFloatField(r, mv, s);
                    r.x += EditorGUIUtility.fieldWidth + 2;
                    nv = EditorGUI.DelayedFloatField(r, nv, s);

                    mp.floatValue = Mathf.Min(Mathf.Max(mv, mm.min), Mathf.Min(nv, mm.max));
                    np.floatValue = Mathf.Max(Mathf.Max(mv, mm.min), Mathf.Min(nv, mm.max));

                    EditorGUI.indentLevel = i;

                    if (mm.colorize)
                    {
                        GUI.color = oldColor;
                    }
                }
            }
        }
    }
}     
