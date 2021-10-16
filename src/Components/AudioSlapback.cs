#region

using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Appalachia.Audio.Components
{
    public class AudioSlapback : Zone
    {
        public static readonly HashSet<AudioSlapback> allSlapbacks = new();

        protected new void OnEnable()
        {
            base.OnEnable();
            allSlapbacks.Add(this);
        }

        protected new void OnDisable()
        {
            allSlapbacks.Remove(this);
            base.OnDisable();
        }

        public static AudioSlapback FindClosest(Vector3 p, out Vector3 rp, out Vector3 rd)
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
}
