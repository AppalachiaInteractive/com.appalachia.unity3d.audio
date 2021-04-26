#include "AudioPluginUtil.h"

namespace ChannelMonitor
{
    const float MAXSAMPLES = 30.0f;
    const int MAXINSTANCES = 8;

    enum Param
    {
        P_INSTANCE,
        P_NUM
    };	

    class LoudnessAnalyzer
    {
    public:
        float peak[8];
        float rms[8];
        float attack;
        float release;
        float updateperiod;
        float updatecount;
        HistoryBuffer peakbuf;
        HistoryBuffer rmsbuf;
    public:
        void Init(float lengthInSeconds, float updateRateInHz, float attackTime, float releaseTime, float samplerate)
        {
            attack = 1.0f - powf(0.01f, 1.0f / (samplerate * attackTime));
            release = 1.0f - powf(0.01f, 1.0f / (samplerate * releaseTime));
            updateperiod = samplerate / updateRateInHz;
            int length = (int)ceilf(lengthInSeconds * updateRateInHz);
            peakbuf.Init(length);
            rmsbuf.Init(length);
        }

        inline void Feed(const float* inputs, int numchannels)
        {
            float maxPeak = 0.0f, maxRMS = 0.0f;
            for (int i = 0; i < numchannels; i++)
            {
                float x = inputs[i];
                x = fabsf(x);
                peak[i] += (x - peak[i]) * ((x > peak[i]) ? attack : release);
                x *= x;
                rms[i] += (x - rms[i]) * ((x > rms[i]) ? attack : release);
                if (peak[i] > maxPeak)
                    maxPeak = peak[i];
                if (rms[i] > maxRMS)
                    maxRMS = rms[i];
            }
            if (--updatecount <= 0.0f)
            {
                updatecount += updateperiod;
                peakbuf.Feed(maxPeak);
                rmsbuf.Feed(sqrtf(maxRMS));
            }
        }

        void ReadBuffer(float* buffer, int numsamplesTarget, float windowLength, float samplerate, bool rms)
        {
            int numsamplesSource = (int)ceilf(samplerate * windowLength / updateperiod);
            HistoryBuffer& buf = (rms) ? rmsbuf : peakbuf;
            buf.ReadBuffer(buffer, numsamplesTarget, numsamplesSource, (float)updatecount / (float)updateperiod);
        }
    };

	struct MonitoringInstance
    {
        LoudnessAnalyzer momentary;
        LoudnessAnalyzer shortterm;
        LoudnessAnalyzer integrated;
    };
	   	    
	 inline MonitoringInstance* GetMonitoringInstance(int index, int samplerate)
    {
        static bool initialized[MAXINSTANCES] = { false };
        static MonitoringInstance instance[MAXINSTANCES];
        if (index < 0 || index >= MAXINSTANCES)
            return NULL;
        if (!initialized[index])
        {
            initialized[index] = true;
			
			instance[index].momentary.Init(3.0f, (float)samplerate, 0.4f, 0.4f, (float)samplerate);
			instance[index].shortterm.Init(MAXSAMPLES, 4.0f, 3.0f, 3.0f, (float)samplerate);
			instance[index].integrated.Init(MAXSAMPLES, 1.0f, 3.0f, 3.0f, (float)samplerate);

        }
        return &instance[index];
    }


    struct EffectData
    {
        float p[P_NUM];
    };

    int InternalRegisterEffectDefinition(UnityAudioEffectDefinition& definition)
    {
        int numparams = P_NUM;
        definition.paramdefs = new UnityAudioParameterDefinition[numparams];
        RegisterParameter(definition, "Instance", "", 0.0f, MAXINSTANCES - 1, 0.0f, 1.0f, 1.0f, P_INSTANCE, "Determines the instance from which the monitoring can occur via ChannelMonitor_Get*");
        return numparams;
    }

    UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK CreateCallback(UnityAudioEffectState* state)
    {
        EffectData* data = new EffectData;
        memset(data, 0, sizeof(EffectData));
        InitParametersFromDefinitions(InternalRegisterEffectDefinition, data->p);
        state->effectdata = data;
        return UNITY_AUDIODSP_OK;
    }

    UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK ReleaseCallback(UnityAudioEffectState* state)
    {
        EffectData* data = state->GetEffectData<EffectData>();
        delete data;
        return UNITY_AUDIODSP_OK;
    }

	UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK ProcessCallback(UnityAudioEffectState* state, float* inbuffer, float* outbuffer, unsigned int length, int inchannels, int outchannels)
    {
        EffectData* data = state->GetEffectData<EffectData>();
		
        memcpy(outbuffer, inbuffer, sizeof(float) * length * inchannels);

        MonitoringInstance* instance = GetMonitoringInstance((int)data->p[P_INSTANCE], state->samplerate);
        if (instance == NULL)
            return UNITY_AUDIODSP_OK;
		        
        const float* src = inbuffer;
        for (unsigned int n = 0; n < length; n++)
        {
            instance->momentary.Feed(src, inchannels);
            instance->shortterm.Feed(src, inchannels);
            instance->integrated.Feed(src, inchannels);
            src += inchannels;
        }
		
		return UNITY_AUDIODSP_OK;
    }

    UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK SetFloatParameterCallback(UnityAudioEffectState* state, int index, float value)
    {
        EffectData* data = state->GetEffectData<EffectData>();
        if (index >= P_NUM)
            return UNITY_AUDIODSP_ERR_UNSUPPORTED;
        data->p[index] = value;
        return UNITY_AUDIODSP_OK;
    }

	UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK GetFloatParameterCallback(UnityAudioEffectState* state, int index, float* value, char *valuestr)
    {
        EffectData* data = state->GetEffectData<EffectData>();
        if (index >= P_NUM)
            return UNITY_AUDIODSP_ERR_UNSUPPORTED;
        if (value != NULL)
            *value = data->p[index];
        if (valuestr != NULL)
            valuestr[0] = 0;
        return UNITY_AUDIODSP_OK;
    }

	int UNITY_AUDIODSP_CALLBACK GetFloatBufferCallback(UnityAudioEffectState* state, const char* name, float* buffer, int numsamples)
    {
        return UNITY_AUDIODSP_OK;
    }

	extern "C" UNITY_AUDIODSP_EXPORT_API float ChannelMonitor_GetLoudness_Momentary(int index)
    {
        MonitoringInstance* instance = GetMonitoringInstance(index, 22050);
        if (instance == NULL)
            return 0.0f;

		return ( instance->momentary.peak[0] + instance->momentary.peak[1] ) / 2.0f;
    }

	extern "C" UNITY_AUDIODSP_EXPORT_API float ChannelMonitor_GetLoudness_ShortTerm(int index)
    {
        MonitoringInstance* instance = GetMonitoringInstance(index, 22050);
        if (instance == NULL)
            return 0.0f;

		return ( instance->shortterm.peak[0] + instance->shortterm.peak[1] ) / 2.0f;
    }

	extern "C" UNITY_AUDIODSP_EXPORT_API float ChannelMonitor_GetLoudness_Integrated(int index)
    {
        MonitoringInstance* instance = GetMonitoringInstance(index, 22050);
        if (instance == NULL)
            return 0.0f;

		return ( instance->integrated.peak[0] + instance->integrated.peak[1] ) / 2.0f;
    }
}
