namespace Appalachia.Audio.Contextual.Context
{
    /// <summary>
    /// </summary>
    public enum WindStrength_AudioContexts : short
    {
        /// <summary>
        ///     Less than 2.5 meters per second.
        /// </summary>
        Calm = 00,

        /// <summary>
        ///     Between 2.5 meters per second and 8.0 meters per second.
        /// </summary>
        LightBreeze = 10,

        /// <summary>
        ///     Between 8.0 meters per second and 13.5 meters per second.
        /// </summary>
        ModerateBreeze = 20,

        /// <summary>
        ///     Between 13.5 meters per second and 19.0 meters per second.
        /// </summary>
        StrongBreeze = 30,

        /// <summary>
        ///     Between 19.0 meters per second and 25.0 meters per second.
        /// </summary>
        Gale = 40,

        /// <summary>
        ///     Greater than 25.0 meters per second.
        /// </summary>
        Storm = 50
    }
}
