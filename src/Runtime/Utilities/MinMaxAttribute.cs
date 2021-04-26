
using UnityEngine;

namespace Internal.Core.Audio {

public class MinMaxAttribute : PropertyAttribute {
    public float min;
    public float max;
    public bool colorize;

    public MinMaxAttribute(float mv, float nv) {
        min = mv;
        max = nv;
    }
}

} // Internal.Core.Audio

