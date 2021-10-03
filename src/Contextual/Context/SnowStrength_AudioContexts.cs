namespace Appalachia.Core.AssetMetadata.AudioMetadata.Context
{
    /// <summary>
    /// </summary>
    public enum SnowStrength_AudioContexts : short
    {
        /// <summary>
        ///     Full visibility.
        /// </summary>
        NoSnow = 0,

        /// <summary>
        ///     Visibility of 1 kilometer (1,100 yd) or greater.
        /// </summary>
        LightSnow = 10,

        /// <summary>
        ///     Visibility between 1 kilometer (1,100 yd) and 0.5 kilometers (550 yd).
        /// </summary>
        ModerateSnow = 20,

        /// <summary>
        ///     Visibility of less than 0.5 kilometers (550 yd).
        /// </summary>
        HeavySnow = 30,

        /// <summary>
        ///     Visibility of less than .25 kilometers.
        /// </summary>
        Blizzard = 40
    }
}
