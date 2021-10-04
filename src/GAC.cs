using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Audio;

namespace Appalachia.Audio
{
    public static partial class GAC
    {
        public const float THRESHOLD_DB_LOW = -80.0F;
        public const float THRESHOLD_DB_HIGH = 0.0F;
        public static float THRESHOLD_LINEAR_LOW = 20.0f * Mathf.Pow(10.0f, THRESHOLD_DB_LOW * 0.05f);
    
        public static float Lookup(AudioMixer mixer, string property)
        {
            if (mixer.GetFloat(property, out var result))
            {
                return result;
            }
            
            throw new NotSupportedException($"Mixer [{mixer.name}] does not have property [{property}].");
        }
    
        [DllImport("AudioPluginDemo")]
        private static extern float ChannelMonitor_GetLoudness(int index);

    
        [DllImport("AudioPluginDemo")]
        private static extern void ChannelMonitor_GetSpectrum(int index, int numsamples, int channel, float[] data);

        [DllImport("AudioPluginDemo")]
        private static extern void ChannelMonitor_GetCorrelation(int index, int numsamples, float[] data);
    
        public static float ChannelMonitor_GetLoudnessData(int index)
        {
            return ChannelMonitor_GetLoudness(index);
        }
    
        public static void ChannelMonitor_GetSpectrumData(int index, int channel, float[] data)
        {
            var numsamples = data.Length;

            ChannelMonitor_GetSpectrum(index, numsamples, channel, data);
        }

        public static void ChannelMonitor_GetCorrelationData(int index, float[] data)
        {
            var numsamples = data.Length;
            ChannelMonitor_GetCorrelation(index, numsamples, data);
        }

        public static float floatTodB(float mag)
        {
            return mag < THRESHOLD_LINEAR_LOW ? THRESHOLD_DB_LOW : 20.0f * Mathf.Log10(mag);
        }

        public static float dBToGain(float dB)
        {
            return Mathf.Pow(10.0f, (dB) / 20.0f);
        }

        public static float dBToNormalized(float dB, float oldMin = THRESHOLD_DB_LOW, float oldMax = THRESHOLD_DB_HIGH, float newMin = 0.0f, float newMax = 1.0f, bool clamp = true)
        {
            var n =  (dB - oldMin) / (oldMax - oldMin);

            if (clamp)
            {
                n = Mathf.Clamp01(n);
            }
        
            return newMin + (n * newMax);
        }

        public static float ChannelMonitor_GetLoudnessData_dB(int index)
        {
            var m = ChannelMonitor_GetLoudnessData(index);
            return floatTodB(m);
        }

        public static void ChannelMonitor_GetSpectrumData_dB(int index, int channel, float[] data)
        {
            ChannelMonitor_GetSpectrumData(index, channel, data);

            for (var i = 0; i < data.Length; i++)
            {
                data[i] = floatTodB(data[i]);
            }
        }

        public static void ChannelMonitor_GetCorrelationData_dB(int index, float[] data)
        {
            ChannelMonitor_GetCorrelationData(index, data);

            for (var i = 0; i < data.Length; i++)
            {
                data[i] = floatTodB(data[i]);
            }
        }
    
        public static class WIND
        {

            public const float WIND_THRESHOLD_LOW_DB = -75.0f;
            public const float WIND_THRESHOLD_HIGH_DB = -20.0f;
        
            //public const string _GUST_CHANNEL_MONITOR_INSTANCE_RAW = "_WIND_GUST_CHANNEL_MONITOR_INSTANCE_RAW";
            public const string _GUST_CHANNEL_MONITOR_INSTANCE_VERYHIGH = "_WIND_GUST_CHANNEL_MONITOR_INSTANCE_VERYHIGH";
            public const string _GUST_CHANNEL_MONITOR_INSTANCE_HIGH = "_WIND_GUST_CHANNEL_MONITOR_INSTANCE_HIGH";
            public const string _GUST_CHANNEL_MONITOR_INSTANCE_MID = "_WIND_GUST_CHANNEL_MONITOR_INSTANCE_MID";
            public const string _GUST_CHANNEL_MONITOR_INSTANCE_LOW = "_WIND_GUST_CHANNEL_MONITOR_INSTANCE_LOW";
        

        }
    }
}
