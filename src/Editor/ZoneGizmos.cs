using Appalachia.Audio.Components;
using UnityEditor;
using UnityEngine;

namespace Appalachia.Audio
{
    namespace Editor
    {
        internal static class ZoneGizmos
        {
            private static bool prefsInited;
            private static bool alwaysShowZoneGizmosPrefs;

            private static void InitPrefs()
            {
                if (!prefsInited)
                {
                    prefsInited = true;
                    alwaysShowZoneGizmosPrefs =
                        EditorPrefs.GetBool("Appalachia.Core.Audio:alwaysShowZoneGizmos");
                }
            }

            [PreferenceItem("Appalachia.Core.Audio")]
            private static void OnPrefsGUI()
            {
                InitPrefs();
                alwaysShowZoneGizmosPrefs = EditorGUILayout.Toggle(
                    "Always Show Zone Gizmos",
                    alwaysShowZoneGizmosPrefs
                );
                if (GUI.changed)
                {
                    EditorPrefs.SetBool(
                        "Appalachia.Core.Audio:alwaysShowZoneGizmos",
                        alwaysShowZoneGizmosPrefs
                    );
                }
            }

            [DrawGizmo(
                GizmoType.Pickable |
                GizmoType.InSelectionHierarchy |
                GizmoType.NotInSelectionHierarchy
            )]
            private static void DrawGizmo(AudioEmitter e, GizmoType t)
            {
                InitPrefs();
                var c = e.gizmo.color;
                if ((t & GizmoType.Selected) != 0)
                {
                    c.a = 0.4f;
                }
                else
                {
                    c.a = 0.1f;
                }

                Gizmos.color = c;
                if (((t & GizmoType.NotInSelectionHierarchy) == 0) || alwaysShowZoneGizmosPrefs)
                {
                    if (!e.GetComponent<AudioZone>())
                    {
                        Gizmos.DrawCube(e.transform.position, Vector3.one * 5f);
                        if ((t & GizmoType.Selected) != 0)
                        {
                            Gizmos.DrawWireCube(e.transform.position, Vector3.one * 5f);
                        }
                    }
                }

                Gizmos.DrawIcon(e.transform.position, "Appalachia.Core.Audio_AudioEmitter.png");
            }

            [DrawGizmo(
                GizmoType.Pickable |
                GizmoType.InSelectionHierarchy |
                GizmoType.NotInSelectionHierarchy
            )]
            private static void DrawGizmo(AudioSlapback s, GizmoType t)
            {
                InitPrefs();
                var c = s.GetGizmoColor();
                if ((t & GizmoType.Selected) != 0)
                {
                    c.a = 0.4f;
                }
                else
                {
                    c.a = 0.1f;
                }

                Gizmos.color = c;
                if (((t & GizmoType.NotInSelectionHierarchy) == 0) || alwaysShowZoneGizmosPrefs)
                {
                    DrawZ(s, 1f, t);
                }

                Gizmos.DrawIcon(s.transform.position, "Appalachia.Core.Audio_AudioSlapback.png");
            }

            [DrawGizmo(
                GizmoType.Pickable |
                GizmoType.InSelectionHierarchy |
                GizmoType.NotInSelectionHierarchy
            )]
            private static void DrawGizmo(AudioZone z, GizmoType t)
            {
                InitPrefs();
                var c = z.GetGizmoColor();
                if ((t & GizmoType.Selected) != 0)
                {
                    c.a = 0.4f;
                }
                else
                {
                    c.a = 0.1f;
                }

                Gizmos.color = c;
                if (z.peripheralFade.min < 1f)
                {
                    Gizmos.color *= 0.5f;
                }

                if (((t & GizmoType.NotInSelectionHierarchy) == 0) || alwaysShowZoneGizmosPrefs)
                {
                    DrawZ(z, z.peripheralFade.min, t);
                }
            }

            private static void DrawZ(Zone z, float inner, GizmoType t)
            {
                var g = z.trigger;
                if (g is BoxCollider)
                {
                    var m = Gizmos.matrix;
                    Gizmos.matrix = z.transform.localToWorldMatrix;
                    Gizmos.DrawCube(Vector3.zero, ((BoxCollider) g).size);
                    if ((t & GizmoType.Selected) != 0)
                    {
                        Gizmos.DrawWireCube(Vector3.zero, ((BoxCollider) g).size);
                    }

                    Gizmos.matrix = m;
                }
                else
                {
                    var r = z.GetRadius();
                    Gizmos.DrawSphere(z.transform.position, r);
                    if (((t & GizmoType.Selected) != 0) || alwaysShowZoneGizmosPrefs)
                    {
                        if (inner < 1f)
                        {
                            Gizmos.color *= 2f;
                            Gizmos.DrawWireSphere(z.transform.position, r * inner);
                            Gizmos.DrawSphere(z.transform.position, r * inner);
                        }
                        else
                        {
                            Gizmos.DrawWireSphere(z.transform.position, r);
                        }
                    }
                }
            }
        }
    } // Editor
}     // Appalachia.Core.Audio
