#region

using System;
using UnityEngine;

#endregion

namespace Appalachia.Audio.Components
{
    [Serializable]
    public struct Envelope
    {
        internal enum Cadence
        {
            Snappy,
            Normal,
            Hesitant
        }

        #region Constants and Static Readonly

        public static readonly Envelope instant = new()
        {
            attackTime = 0f,
            attackValue = 1f,
            releaseTime = 0f,
            releaseValue = 0f,
            sustainValue = 1f
        };

        #endregion

        #region Fields and Autoproperties

        public float attackTime;

        public float releaseTime;

        public float sustainValue;
        internal Cadence attackCadence;
        internal float attackValue;
        internal float releaseValue;

        #endregion

        public float GetAttackValue()
        {
            return attackValue;
        }

        public float GetReleaseValue()
        {
            return releaseValue;
        }

        public float GetValue()
        {
            var a = attackValue;
            var r = releaseValue;
            if (attackCadence == Cadence.Snappy)
            {
                a = ((a - 1f) * (a - 1f) * (a - 1f)) + 1f;
            }
            else if (attackCadence == Cadence.Hesitant)
            {
                a = a * a;
            }

            r = 1f - (((r - 1f) * (r - 1f) * (r - 1f)) + 1f);
            return a * r * sustainValue;
        }

        public void SetAttack(float t)
        {
            attackTime = t;
            attackValue = 0f;
            attackCadence = t <= 1f
                ? Cadence.Snappy
                : t <= 10f
                    ? Cadence.Normal
                    : Cadence.Hesitant;
        }

        public void SetRelease(float t)
        {
            releaseTime = t;
            releaseValue = 0f;
        }

        public float UpdateAttack(float dt)
        {
            var speed = attackTime > 0f ? 1f / attackTime : Mathf.Infinity;
            return attackValue = Mathf.Clamp01(attackValue + (dt * speed));
        }

        public float UpdateRelease(float dt)
        {
            var speed = releaseTime > 0f ? 1f / releaseTime : Mathf.Infinity;
            return releaseValue = Mathf.Clamp01(releaseValue + (dt * speed));
        }
    }
}
