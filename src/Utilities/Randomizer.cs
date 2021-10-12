namespace Appalachia.Audio.Utilities
{
    public static class Randomizer
    {
        public const float denominator = 1f / 0x80000000;

        public static int seed;

        public static int next
        {
            get { return seed = (seed + 35757) * 31313; }
        }

        public static float plusMinusOne => next * denominator;
        public static float zeroToOne => (plusMinusOne + 1f) * 0.5f;
    }
} 
