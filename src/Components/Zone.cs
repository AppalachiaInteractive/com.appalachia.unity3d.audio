#region

using System;
using System.Collections.Generic;
using Appalachia.Audio.Utilities;
using UnityEngine;

#endregion

namespace Appalachia.Audio.Components
{
    public abstract class Zone : MonoBehaviour
    {
        #region Constants and Static Readonly

        public static readonly List<Zone> allZones = new();

        #endregion

        #region Fields

        public static bool dontProbeZones;

        [Range(0, 1)] public float parentExclusion;

        public float radius = 5f;
        internal bool inited;
        internal bool wantActive;

        internal float volumeExclusion;
        internal float volumeInfluence;

        internal int hash;
        internal List<Zone> children;
        internal TernaryBool active;

        internal Zone parent;

        protected Collider _trigger;
        protected int _triggerRefs;

        #endregion

        public bool isVolumeExcluded => volumeExclusion > 0f;

        public Collider trigger => inited ? _trigger : FindTrigger(this);

        #region Event Functions

        protected void OnEnable()
        {
            if (!inited)
            {
                OnInit();
            }

            allZones.Add(this);
            OnUpdateActivation(wantActive);
        }

        protected void OnDisable()
        {
            allZones.Remove(this);
            OnUpdateActivation(false);
            _triggerRefs = 0;
        }

        #endregion

        protected virtual void OnInit()
        {
            if ((_trigger = FindTrigger(this)) == null)
            {
                RegisterWithParentZone();
            }

            hash = (int) Synthesizer.GetNextHandle();
            inited = true;
        }

        protected virtual void OnProbe(Vector3 lpos, int thisFrame)
        {
        }

        protected virtual void OnUpdateEmitters()
        {
        }

        public static Collider FindTrigger(Zone z)
        {
            var c = z.GetComponent<Collider>();
            return (c != null) && c.isTrigger ? c : null;
        }

        internal static void UpdateZone(int thisFrame)
        {
            UpdateProbes(thisFrame);
            UpdateActivation();
            UpdateInfluence();
            UpdateExclusion();
            UpdateEmitters();
        }

        private static float GetTriggerRadius(Collider c)
        {
            var e = c.bounds.extents;
            return Mathf.Max(e.x, e.z);
        }

        private static void UpdateActivation()
        {
            _10:
            foreach (var z in allZones)
            {
                if (z.OnUpdateActivation(z.wantActive))
                {
                    goto _10;
                }
            }
        }

        private static void UpdateEmitters()
        {
            foreach (var z in allZones)
            {
                z.OnUpdateEmitters();
            }
        }

        private static void UpdateExclusion()
        {
            foreach (var z in allZones)
            {
                if ((z.active == true) && (z.parent == null))
                {
                    float e;
                    UpdateExclusionDepthFirst(z, out e);
                }
            }
        }

        private static void UpdateExclusionDepthFirst(Zone z, out float exclusion)
        {
            // calculate max exclusion of all children
            var e = 0f;
            if (z.children != null)
            {
                foreach (var c in z.children)
                {
                    float f;
                    UpdateExclusionDepthFirst(c, out f);
                    e = Mathf.Max(e, f);
                }
            }

            // keep track of our own exclusion
            z.volumeExclusion = e;

            // apply our own influence and exclusion and pass it up to parent
            var k = e + 1f;
            k *= (z.volumeInfluence * z.parentExclusion) + 1f;
            exclusion = Mathf.Clamp01(k - 1f);
        }

        private static void UpdateInfluence()
        {
            foreach (var z in allZones)
            {
                // default is full influence and no exclusion
                z.volumeExclusion = 0f;
                z.volumeInfluence = z.active == true ? 1f : 0f;

                // for active audio zones with peripheral fade, adjust the influence
                AudioZone az;
                if ((z.active == true) && ((az = z as AudioZone) != null))
                {
                    var pfMin = az.peripheralFade.min;
                    var pfMax = az.peripheralFade.max;
                    if (pfMin < 1f)
                    {
                        var x = az.sqrDistance / az.sqrRadius;
                        var y = 1f - Mathf.Clamp01((x - pfMin) / (pfMax - pfMin));
                        az.volumeInfluence = y * y;
                    }
                }
            }
        }

        private static void UpdateProbes(int thisFrame)
        {
            if (!dontProbeZones)
            {
                var l = Heartbeat.listenerTransform;
                if (l)
                {
                    var lpos = l.position;
                    foreach (var z in allZones)
                    {
                        z.OnProbe(lpos, thisFrame);
                    }
                }
            }
        }

        public Zone FindParentZone()
        {
            return FindParentZoneRecursive(transform.parent);
        }

        public float GetRadius()
        {
            var t = trigger;
            return t ? GetTriggerRadius(t) : radius;
        }

        protected bool OnUpdateActivation(bool state)
        {
            if (active == state)
            {
                return false;
            }

            active = state;
            if (children != null)
            {
                foreach (var i in children)
                {
                    i.enabled = state;
                }
            }

            return true;
        }

        protected void SetActive(bool state)
        {
            wantActive = state;
        }

        private Zone FindParentZoneRecursive(Transform t)
        {
            if (t != null)
            {
                var z = t.GetComponent<Zone>();
                return z != null ? z : FindParentZoneRecursive(t.parent);
            }

            return null;
        }

        private void RegisterWithParentZone()
        {
            var z = FindParentZone();
            if (z != null)
            {
                if (z.children == null)
                {
                    z.children = new List<Zone>(4);
                }

                z.children.Add(this);
                parent = z;
            }
        }

#if UNITY_EDITOR
        [Serializable]
        public struct GizmoParams
        {
            #region Fields

            public Color activeColor;
            public Color inactiveColor;

            #endregion
        }

        [Colorize] public GizmoParams gizmo = new() {activeColor = Color.magenta, inactiveColor = Color.blue};
#endif

#if UNITY_EDITOR
        public bool IsActive()
        {
            return active == true;
        }

        public Color GetGizmoColor()
        {
            return IsActive() ? gizmo.activeColor : gizmo.inactiveColor;
        }
#endif
    }
}
