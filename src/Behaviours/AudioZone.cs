#region

using System.Text;
using Appalachia.Audio.Utilities;
using Appalachia.CI.Integration.Assets;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Appalachia.Audio.Behaviours
{
    [ExecuteAlways]
    public class AudioZone : Zone
    {
        public enum Ownership
        {
            Local,
            Deep
        }

        #region Fields and Autoproperties

        [MinMaxSlider(0, 1, true)] public Vector2 peripheralFade = new() { x = 1f, y = 1f };
        public LayerMask layerMask = -1;

        public Ownership ownership;

        [ShowInInspector]
        internal AudioEmitter[] emitters;
        internal float sqrDistance;
        internal float sqrRadius;
        internal int lastFrame;

        private TernaryBool hasEnabledEmitters;

        #endregion

        public bool hasPeripheralFade => peripheralFade.x < 1f;

        #region Event Functions

        protected new void OnEnable()
        {
            base.OnEnable();
            lastFrame = -1;
        }

        protected new void OnDisable()
        {
            for (int i = 0, n = emitters.Length; i < n; ++i)
            {
                emitters[i].enabled = false;
            }

            base.OnDisable();
        }

        protected void OnTriggerEnter(Collider c)
        {
            if ((_trigger != null) && ((layerMask & (1 << c.gameObject.layer)) != 0) && (_triggerRefs++ == 0))
            {
                SetActive(true);
            }
        }

        protected void OnTriggerExit(Collider c)
        {
            if ((_trigger != null) && ((layerMask & (1 << c.gameObject.layer)) != 0) && (--_triggerRefs == 0))
            {
                SetActive(false);
            }
        }

        #endregion

        public static AudioEmitter[] FindEmitters(AudioZone z)
        {
            return z.ownership == Ownership.Local
                ? z.GetComponents<AudioEmitter>()
                : z.GetComponentsInChildren<AudioEmitter>();
        }

        protected override void OnInit()
        {
            base.OnInit();
            emitters = FindEmitters(this);
            for (int i = 0, n = emitters.Length; i < n; ++i)
            {
                emitters[i].zone = this;
            }
        }

        protected override void OnProbe(Vector3 lpos, int thisFrame)
        {
            if ((_trigger == null) && (lastFrame != thisFrame))
            {
                lastFrame = thisFrame;

                var pos = transform.position;
                sqrDistance = (lpos - pos).sqrMagnitude;
                sqrRadius = radius * radius;
                SetActive(sqrDistance <= sqrRadius);
            }
        }

        protected override void OnUpdateEmitters()
        {
            var wantEnabledEmitters = (active == true) && (volumeExclusion < 1f);
            if (hasEnabledEmitters != wantEnabledEmitters)
            {
                hasEnabledEmitters = wantEnabledEmitters;
                for (int i = 0, n = emitters.Length; i < n; ++i)
                {
                    emitters[i].enabled = wantEnabledEmitters;
                }
            }
        }

#if UNITY_EDITOR

        #region Static Fields and Autoproperties

        protected static StringBuilder builder = new();

        #endregion

        #region Event Functions

        protected void OnSceneGUI()
        {
            var z = (Zone)this;
            var p = z.transform.position;

            DrawZoneLabel(z, p);

            var u = z.FindParentZone();
            if (u != null)
            {
                var v = u.transform.position;
                if (u is AudioZone)
                {
                    DrawZoneLabel((AudioZone)u, v);
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
                var e = FindEmitters(zone);
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

            UnityEditor.Handles.BeginGUI();
            var c = GUI.color;
            GUI.color = Color.white;
            var x = new GUIContent(GetZoneCaption(z));
            var y = new GUIContent(s);
            var l = UnityEditor.HandleUtility.WorldPointToSizedRect(p, x, UnityEditor.EditorStyles.boldLabel);
            var m = UnityEditor.HandleUtility.WorldPointToSizedRect(p, y, UnityEditor.EditorStyles.helpBox);
            var n = Mathf.Max(l.width, m.width);
            l.width = n;
            m.width = n;
            l.x -= m.width * 1.2f;
            l.y -= m.height * 0.5f;
            m.x = l.x - Mathf.Max(0f, m.width - l.width);
            m.y = l.y + 1f;
            UnityEditor.EditorGUI.HelpBox(m, y.text, UnityEditor.MessageType.None);
            UnityEditor.EditorGUI.DropShadowLabel(l, x);
            GUI.color = c;
            UnityEditor.Handles.EndGUI();

            if (z.trigger == null)
            {
                UnityEditor.EditorGUI.BeginChangeCheck();
                var f = UnityEditor.Handles.RadiusHandle(z.transform.rotation, p, z.radius);
                if (UnityEditor.EditorGUI.EndChangeCheck())
                {
                    UnityEditor.Undo.RecordObject(z, "Changed Zone Radius");
                    z.radius = f;
                }
            }
        }

        protected override void DrawZoneLabel(Zone z, Vector3 p)
        {
            DrawZoneLabelStatic(z, p);
        }

        protected static string GetZoneCaption(Zone z)
        {
            return $"{z.name} ({z.GetRadius():N2})";
        }

        #region Menu Items

        [UnityEditor.MenuItem(PKG.Menu.GameObjects.Base + nameof(AudioZone))]
        private static void CreateAudioZone()
        {
            var o = new GameObject("Audio Zone");
            o.AddComponent<AudioZone>();

            var p = UnityEditor.Selection.activeGameObject;
            if ((p != null) && !AssetDatabaseManager.IsMainAsset(p) && !AssetDatabaseManager.IsSubAsset(p))
            {
                o.transform.parent = p.transform;
            }

            UnityEditor.EditorGUIUtility.PingObject(o);
        }

        #endregion

#endif
    }
}
