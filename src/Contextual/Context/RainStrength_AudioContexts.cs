namespace Appalachia.Audio.Contextual.Context
{
    /// <summary>
    ///     Rainfall intensity is classified according to the rate of precipitation, which depends on the considered time.
    ///     The following categories are used to classify rainfall intensity:
    ///     https://en.wikipedia.org/wiki/Rain#Characteristics
    /// </summary>
    public enum RainStrength_AudioContexts : short
    {
        /// <summary>
        ///     When precipitation rate is at 0 mm per hour.
        /// </summary>
        NoRain = 0,

        /// <summary>
        ///     When the precipitation rate is less than 2.5 mm (0.098 in) per hour.
        /// </summary>
        LightRain = 10,

        /// <summary>
        ///     When the precipitation rate is between 2.5 mm (0.098 in) - 7.6 mm (0.30 in) or 10 mm (0.39 in) per hour.
        /// </summary>
        ModerateRain = 20,

        /// <summary>
        ///     When the precipitation rate is > 7.6 mm (0.30 in) per hour,[106] or between 10 mm (0.39 in) and 50 mm (2.0 in) per hour.
        /// </summary>
        HeavyRain = 30,

        /// <summary>
        ///     When the precipitation rate is > 50 mm (2.0 in) per hour.
        /// </summary>
        ViolentRain = 40
    }
}
