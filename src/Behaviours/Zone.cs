#region

using System;
using System.Collections.Generic;
using Appalachia.Audio.Core;
using Appalachia.Audio.Utilities;
using Appalachia.Core.Objects.Initialization;
using Appalachia.Core.Objects.Root;
using Appalachia.Utility.Async;
using Sirenix.OdinInspector;
using Unity.Profiling;
using UnityEngine;

#endregion

namespace Appalachia.Audio.Behaviours
{
    [ExecuteAlways]
    public abstract class Zone<T> : AppalachiaBehaviour<T>
        where T : Zone<T>
    {
        #region Constants and Static Readonly

        public static readonly List<T> allZones = new();

        #endregion

        #region Static Fields and Autoproperties

        // ReSharper disable once StaticMemberInGenericType
        public static bool dontProbeZones;

        #endregion

        #region Fields and Autoproperties

        [PropertyRange(0, 1)] public float parentExclusion;

        public float radius = 5f;
        internal bool hasBeenInitialized;
        internal bool wantActive;

        internal float volumeExclusion;
        internal float volumeInfluence;

        internal int hash;
        internal List<T> children;
        internal TernaryBool active;

        internal T parent;

        protected Collider _trigger;
        protected int _triggerRefs;

        #endregion

        public bool isVolumeExcluded => volumeExclusion > 0f;

        public Collider trigger => hasBeenInitialized ? _trigger : FindTrigger(this as T);

        #region Event Functions

        protected override async AppaTask WhenDisabled()

        {
            
            {
                await base.WhenDisabled();

                allZones.Remove(this as T);
                OnUpdateActivation(false);
                _triggerRefs = 0;
            }
        }

        #endregion

        public static Collider FindTrigger(T z)
        {
            var c = z.GetComponent<Collider>();
            return (c != null) && c.isTrigger ? c : null;
        }

        public T FindParentZone()
        {
            return FindParentZoneRecursive(transform.parent);
        }

        public float GetRadius()
        {
            var t = trigger;
            return t ? GetTriggerRadius(t) : radius;
        }

        internal static void UpdateZone(int thisFrame)
        {
            UpdateProbes(thisFrame);
            UpdateActivation();
            UpdateInfluence();
            UpdateExclusion();
            UpdateEmitters();
        }

        protected virtual void OnInit()
        {
            if ((_trigger = FindTrigger(this as T)) == null)
            {
                RegisterWithParentZone();
            }

            hash = (int)Synthesizer.GetNextHandle();
            hasBeenInitialized = true;
        }

        protected virtual void OnProbe(Vector3 lpos, int thisFrame)
        {
        }

        protected virtual void OnUpdateEmitters()
        {
        }

        protected override async AppaTask Initialize(Initializer initializer)
        {
            using (_PRF_Initialize.Auto())
            {
                await base.Initialize(initializer);

                if (!hasBeenInitialized)
                {
                    OnInit();
                    await AppaTask.Yield();
                }

                allZones.Add(this as T);

                await AppaTask.Yield();
                
                OnUpdateActivation(wantActive);
            }
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

        private static void UpdateExclusionDepthFirst(Zone<T> z, out float exclusion)
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
                    var pfMin = az.peripheralFade.x;
                    var pfMax = az.peripheralFade.y;
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

        private static T FindParentZoneRecursive(Transform t)
        {
            while (true)
            {
                if (t != null)
                {
                    var z = t.GetComponent<T>();
                    if (z != null)
                    {
                        return z;
                    }

                    t = t.parent;
                    continue;
                }

                return null;
                break;
            }
        }

        private void RegisterWithParentZone()
        {
            var z = FindParentZone();
            if (z != null)
            {
                if (z.children == null)
                {
                    z.children = new List<T>(4);
                }

                z.children.Add(this as T);
                parent = z;
            }
        }

        #region Profiling

        private const string _PRF_PFX = nameof(Zone<T>) + ".";

        private static readonly ProfilerMarker _PRF_Initialize =
            new ProfilerMarker(_PRF_PFX + nameof(Initialize));

        private static readonly ProfilerMarker _PRF_OnDisable =
            new ProfilerMarker(_PRF_PFX + nameof(OnDisable));

        #endregion

#if UNITY_EDITOR
        [Serializable]
        public struct GizmoParams
        {
            #region Fields and Autoproperties

            public Color activeColor;
            public Color inactiveColor;

            #endregion
        }

        public GizmoParams gizmo = new() { activeColor = Color.magenta, inactiveColor = Color.blue };
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

        protected virtual void DrawZoneLabel(T z, Vector3 p)
        {
        }
#endif
    }
}
