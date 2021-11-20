namespace Appalachia.Audio.Utilities
{
    public static class Randomizer
    {
        #region Constants and Static Readonly

        public const float denominator = 1f / 0x80000000;

        #endregion

        #region Static Fields and Autoproperties

        public static int seed;

        #endregion

        public static float plusMinusOne => next * denominator;
        public static float zeroToOne => (plusMinusOne + 1f) * 0.5f;

        public static int next
        {
            get { return seed = (seed + 35757) * 31313; }
        }
    }
}
