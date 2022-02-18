#region

using System.Collections.Generic;
using Appalachia.CI.Integration.Assets;
using Appalachia.Core.Objects.Initialization;
using Appalachia.Utility.Async;
using Unity.Profiling;
using UnityEngine;

#endregion

namespace Appalachia.Audio.Behaviours
{
    public class AudioSlapback : Zone<AudioSlapback>
    {
        #region Constants and Static Readonly

        public static readonly HashSet<AudioSlapback> allSlapbacks = new();

        #endregion

        public static AudioSlapback FindClosest(Vector3 p, out Vector3 rp, out Vector3 rd)
        {
            using (_PRF_FindClosest.Auto())
            {
                var lp = Heartbeat.listenerTransform.position;
                var dp = Mathf.Infinity;
                AudioSlapback z = null;

                rp = Vector3.zero;
                rd = Vector3.zero;
                p.y = 0f;

                foreach (var i in allSlapbacks)
                {
                    var t = i.transform;
                    var qq = t.position;
                    var q = qq;
                    q.y = 0f;

                    var d = q - p;
                    var e = d.sqrMagnitude;
                    if (dp <= e)
                    {
                        continue;
                    }

                    var fwd = t.forward;
                    if (Vector3.Dot(d.normalized, fwd) >= 0f)
                    {
                        continue;
                    }

                    var ld = q - lp;
                    if (Vector3.Dot(ld.normalized, fwd) >= 0f)
                    {
                        continue;
                    }

                    dp = e;
                    rp = qq;
                    rd = d;
                    z = i;
                }

                return z;
            }
        }

        /// <inheritdoc />
        protected override async AppaTask Initialize(Initializer initializer)
        {
            await base.Initialize(initializer);

            using (_PRF_Initialize.Auto())
            {
                allSlapbacks.Add(this);
            }
        }

        /// <inheritdoc />
        protected override async AppaTask WhenDisabled()
        {
            await base.WhenDisabled();

            using (_PRF_WhenDisabled.Auto())
            {
                allSlapbacks.Remove(this);
            }
        }

        #region Profiling

        private static readonly ProfilerMarker _PRF_FindClosest =
            new ProfilerMarker(_PRF_PFX + nameof(FindClosest));

        #endregion

#if UNITY_EDITOR

        /// <inheritdoc />
        protected override void DrawZoneLabel(AudioSlapback z, Vector3 p)
        {
            UnityEditor.Handles.BeginGUI();
            var c = GUI.color;
            GUI.color = Color.white;
            var x = new GUIContent(z.name);
            var y = GUIContent.none;
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
                    UnityEditor.Undo.RecordObject(z, "Changed Slapback Radius");
                    z.radius = f;
                }
            }
        }

        #region Menu Items

        [UnityEditor.MenuItem(PKG.Menu.Appalachia.Components.Base + nameof(AudioSlapback))]
        private static void CreateAudioSlapback()
        {
            var o = new GameObject("Audio Slapback");
            o.AddComponent<AudioSlapback>();

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
