using UnityEngine;

namespace Appalachia.Audio.Utilities
{
    public class FloatRangeAttribute : PropertyAttribute
    {
        public FloatRangeAttribute(float mv, float nv)
        {
            min = mv;
            max = nv;
        }

        public bool colorize;
        public float max;
        public float min;
    }
}
