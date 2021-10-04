
namespace Appalachia.Audio.Utilities {

public static class Randomizer {
    public const float denominator = 1f / (float) 0x80000000;

    public static int seed;

    public static int next { get { return seed = (seed + 35757) * 31313; }}
    public static float plusMinusOne { get { return (float) next * denominator; }}
    public static float zeroToOne { get { return (plusMinusOne + 1f) * 0.5f; }}
}

} // Appalachia.Core.Audio

