using System.Text;
using Appalachia.Audio.Components;
using Appalachia.CI.Integration.Assets;
using UnityEditor;
using UnityEngine;

namespace Appalachia.Audio
{
    [CustomEditor(typeof(AudioZone))]
    public class AudioZoneEditor : UnityEditor.Editor
    {
        #region Static Fields and Autoproperties

        protected static StringBuilder builder = new();

        #endregion

        #region Event Functions

        protected void OnSceneGUI()
        {
            var z = (Zone) target;
            var p = z.transform.position;

            DrawZoneLabel(z, p);

            var u = z.FindParentZone();
            if (u != null)
            {
                var v = u.transform.position;
                if (u is AudioZone)
                {
                    DrawZoneLabel((AudioZone) u, v);
                }
            }

            var q = z.transform.parent;
            if (q != null)
            {
                for (int i = 0, n = q.childCount; i < n; ++i)
                {
                    var v = q.GetChild(i).GetComponents<AudioZone>();
                    for (int j = 0, m = v.Length; j < m; ++j)
                    {
                        if (v[j] && (v[j] != z))
                        {
                            DrawZoneLabel(v[j], v[j].transform.position);
                        }
                    }
                }
            }
        }

        #endregion

        public static void DrawZoneLabelStatic(Zone z, Vector3 p)
        {
            if (z is AudioZone zone)
            {
                var e = AudioZone.FindEmitters(zone);
                if (e.Length > 0)
                {
                    var first = true;
                    foreach (var i in e)
                    {
                        if (i.patches != null)
                        {
                            for (int j = 0, k = i.patches.Length; j < k; ++j)
                            {
                                if (i.patches[j])
                                {
                                    if (first)
                                    {
                                        first = false;
                                        builder.Append('\n');
                                    }

                                    builder.Append('\n');
                                    builder.Append(i.patches[j].name);
                                }
                            }
                        }
                    }
                }
            }

            var s = builder.ToString();
            builder.Length = 0;

            Handles.BeginGUI();
            var c = GUI.color;
            GUI.color = Color.white;
            var x = new GUIContent(GetZoneCaption(z));
            var y = new GUIContent(s);
            var l = HandleUtility.WorldPointToSizedRect(p, x, EditorStyles.boldLabel);
            var m = HandleUtility.WorldPointToSizedRect(p, y, EditorStyles.helpBox);
            var n = Mathf.Max(l.width, m.width);
            l.width = n;
            m.width = n;
            l.x -= m.width * 1.2f;
            l.y -= m.height * 0.5f;
            m.x = l.x - Mathf.Max(0f, m.width - l.width);
            m.y = l.y + 1f;
            EditorGUI.HelpBox(m, y.text, MessageType.None);
            EditorGUI.DropShadowLabel(l, x);
            GUI.color = c;
            Handles.EndGUI();

            if (z.trigger == null)
            {
                EditorGUI.BeginChangeCheck();
                var f = Handles.RadiusHandle(z.transform.rotation, p, z.radius);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(z, "Changed Zone Radius");
                    z.radius = f;
                }
            }
        }

        public override void OnInspectorGUI()
        {
            ColorizeDrawer.Reset();
            var oldColor = GUI.color;
            GUI.color = ColorizeDrawer.GetColor("");

            serializedObject.Update();
            var prop = serializedObject.GetIterator();
            var targ = (AudioZone) serializedObject.targetObject;
            bool disabled;
            if (prop.NextVisible(true))
            {
                do
                {
                    disabled = !((prop.name != "m_Script") &&
                                 ((prop.name != "radius") || (targ.trigger == null)) &&
                                 ((prop.name != "layerMask") || (targ.trigger != null)));
                    EditorGUI.BeginDisabledGroup(disabled);
                    EditorGUILayout.PropertyField(prop);
                    EditorGUI.EndDisabledGroup();
                } while (prop.NextVisible(false));
            }

            serializedObject.ApplyModifiedProperties();

            GUI.color = oldColor;
        }

        protected virtual void DrawZoneLabel(Zone z, Vector3 p)
        {
            DrawZoneLabelStatic(z, p);
        }

        protected static string GetZoneCaption(Zone z)
        {
            return $"{z.name} ({z.GetRadius():N2})";
        }

        #region Menu Items

        [MenuItem(PKG.Menu.Appalachia.Components.Base + nameof(AudioZone))]
        private static void CreateAudioZone()
        {
            var o = new GameObject("Audio Zone");
            o.AddComponent<AudioZone>();

            var p = Selection.activeGameObject;
            if ((p != null) && !AssetDatabaseManager.IsMainAsset(p) && !AssetDatabaseManager.IsSubAsset(p))
            {
                o.transform.parent = p.transform;
            }

            EditorGUIUtility.PingObject(o);
        }

        #endregion
    }
}
