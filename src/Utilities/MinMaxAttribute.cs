
using UnityEngine;

namespace Appalachia.Core.Audio.Utilities {

public class MinMaxAttribute : PropertyAttribute {
    public float min;
    public float max;
    public bool colorize;

    public MinMaxAttribute(float mv, float nv) {
        min = mv;
        max = nv;
    }
}

} // Appalachia.Core.Audio

