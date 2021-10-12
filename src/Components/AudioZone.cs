#region

using Appalachia.Audio.Utilities;
using UnityEngine;

#endregion

namespace Appalachia.Audio.Components
{
    public class AudioZone : Zone
    {
        public enum Ownership
        {
            Local,
            Deep
        }

        public Ownership ownership;
        public LayerMask layerMask = -1;

        [MinMax(0, 1)] public MinMaxFloat peripheralFade = new() {min = 1f, max = 1f};

        internal AudioEmitter[] emitters;

        private TernaryBool hasEnabledEmitters;
        internal int lastFrame;
        internal float sqrDistance;
        internal float sqrRadius;

        public bool hasPeripheralFade => peripheralFade.min < 1f;

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
            if ((_trigger != null) &&
                ((layerMask & (1 << c.gameObject.layer)) != 0) &&
                (_triggerRefs++ == 0))
            {
                SetActive(true);
            }
        }

        protected void OnTriggerExit(Collider c)
        {
            if ((_trigger != null) &&
                ((layerMask & (1 << c.gameObject.layer)) != 0) &&
                (--_triggerRefs == 0))
            {
                SetActive(false);
            }
        }

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
    }
} 
