#include "AudioPluginUtil.h"

namespace ChannelMonitor
{
	const float MAX_LOUDNESS_SAMPLES = 30.0f;
	const int MAX_INSTANCES = 16;
	const int FFTSIZE = 8192;

	enum Param
	{
		P_INSTANCE,
		P_ANALYSIS_WINDOW,
		P_SPECTRUM_DECAY,
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

		inline void Feed(const float* inbuffer, int inchannels)
		{
			float maxPeak = 0.0f, maxRMS = 0.0f;
			for (int i = 0; i < inchannels; i++)
			{
				float x = inbuffer[i];
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

	class CorrelationAnalyzer
	{
	public:
		HistoryBuffer history[8];
	public:
		void Init(float samplerate)
		{
			for (int i = 0; i < 8; i++)
			{
				history[i].Init(samplerate * 2);
			}
		}

		inline void Feed(const float* inbuffer, int inchannels )
		{
			for (int i = 0; i < inchannels; i++)
			{
				float x = inbuffer[i];
				history[i].Feed(x);
			}
		}
	};

	class SpectrumAnalyzer
	{
	public:
		HistoryBuffer history[8];
		HistoryBuffer spectrum[8];
		UnityComplexNumber fftbuf[FFTSIZE];
		float smoothspec[8][FFTSIZE];
	public:
		void Init(float samplerate)
		{
			for (int i = 0; i < 8; i++)
			{
				history[i].Init(samplerate * 2);
				spectrum[i].Init(samplerate * 2);
			}
		}

		inline void Feed(const float* inbuffer, unsigned int length, int inchannels, float samplerate, float decay)
		{
			for (int i = 0; i < inchannels; i++)
			{
				float x = inbuffer[i];
				history[i].Feed(x);
			}

			for (int i = 0; i < inchannels; i++)
			{
				int windowsize = FFTSIZE / 2;
				int w = history[i].writeindex;
				float c = 1.0f, s = 0.0f, f = 2.0f * sinf(kPI / (float)windowsize);
				memset(fftbuf, 0, sizeof(UnityComplexNumber) * FFTSIZE);
				for (int n = 0; n < windowsize; n++)
				{
					fftbuf[n].re = history[i].data[w] * (0.5f - 0.5f * c);
					s += c * f;
					c -= s * f;
					if (--w < 0)
						w = history[i].length - 1;
				}
				FFT::Forward(fftbuf, FFTSIZE, true);
				float specdecay = powf(10.0f, 0.05f * decay * length / (float)samplerate);
				for (int n = 0; n < FFTSIZE / 2; n++)
				{
					float a = fftbuf[n].Magnitude();
					smoothspec[i][n] = (a > smoothspec[i][n]) ? a : smoothspec[i][n] * specdecay;
				}
				spectrum[i].Feed(smoothspec[i], FFTSIZE / 2, 1);
			}
		}
	};

	struct MonitoringInstance
	{
		LoudnessAnalyzer loudness;
		SpectrumAnalyzer spectrum;
		CorrelationAnalyzer correlation;
	};

	inline MonitoringInstance* GetMonitoringInstance(int index, int samplerate)
	{
		static bool initialized[MAX_INSTANCES] = { false };
		static MonitoringInstance instance[MAX_INSTANCES];
		if (index < 0 || index >= MAX_INSTANCES)
			return NULL;
		if (!initialized[index])
		{
			initialized[index] = true;

			instance[index].loudness.Init(MAX_LOUDNESS_SAMPLES, 1.0f, 3.0f, 3.0f, (float)samplerate);
			instance[index].spectrum.Init((float)samplerate);			
			instance[index].correlation.Init((float)samplerate);

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
		RegisterParameter(definition, "Instance", "", 0.0f, MAX_INSTANCES - 1, 0.0f, 1.0f, 1.0f, P_INSTANCE, "Determines the instance from which the monitoring can occur via ChannelMonitor_Get*");
		RegisterParameter(definition, "Analysis Window", "s", 0.01f, 2.0f, 0.1f, 1.0f, 3.0f, P_ANALYSIS_WINDOW, "Length of analysis window");
		RegisterParameter(definition, "Spectrum Decay", "dB/s", -100.0f, 0.0f, -10.0f, 1.0f, 1.0f, P_SPECTRUM_DECAY, "Hold time for overlaid spectra");
		return numparams;
	}

	UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK CreateCallback(UnityAudioEffectState* state)
	{
		EffectData* data = new EffectData;
		memset(data, 0, sizeof(EffectData));
		InitParametersFromDefinitions(InternalRegisterEffectDefinition, data->p);
		state->effectdata = data;

		MonitoringInstance* instance = GetMonitoringInstance((int)data->p[P_INSTANCE], state->samplerate);

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

		for (unsigned int n = 0; n < length; n++)
            for (int i = 0; i < inchannels; i++)
                outbuffer[n * inchannels + i] = inbuffer[n * inchannels + i];
				
		MonitoringInstance* instance = GetMonitoringInstance((int)data->p[P_INSTANCE], state->samplerate);
		if (instance == NULL)
			return UNITY_AUDIODSP_OK;

		const float* src = inbuffer;
		for (unsigned int n = 0; n < length; n++)
		{
			instance->loudness.Feed(src, inchannels);
			//instance->spectrum.Feed(src, length, inchannels, state->samplerate, data->p[P_SPECTRUM_DECAY]);			
            instance->correlation.Feed(src, inchannels);

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


	extern "C" UNITY_AUDIODSP_EXPORT_API float ChannelMonitor_GetLoudness(int index)
	{
		MonitoringInstance* instance = GetMonitoringInstance(index, 22050);

		if (instance == NULL)
			return 0.0f;

		return (instance->loudness.peak[0] + instance->loudness.peak[1]) / 2.0f;
	}

	extern "C" UNITY_AUDIODSP_EXPORT_API void ChannelMonitor_GetSpectrum(int index, int numsamples, int channel, float* data)
	{
		MonitoringInstance* instance = GetMonitoringInstance(index, 22050);

		if (instance == NULL)
			return;

		instance->spectrum.spectrum[channel].ReadBuffer(data, numsamples, FFTSIZE / 2, 0.0f);
	}

	extern "C" UNITY_AUDIODSP_EXPORT_API void ChannelMonitor_GetCorrelation(int index, int numsamples, float* data)
	{
		MonitoringInstance* instance = GetMonitoringInstance(index, 22050);

		if (instance == NULL)
			return;

		HistoryBuffer& l = instance->correlation.history[0];
		HistoryBuffer& r = instance->correlation.history[1];
		int w1 = l.writeindex;
		int w2 = r.writeindex;
		for (int n = 0; n < numsamples / 2; n++)
		{
			data[n * 2 + 0] = l.data[w1];
			if (--w1 < 0)
				w1 = l.length - 1;
			if (n * 2 + 1 < numsamples)
				data[n * 2 + 1] = r.data[w2];
			if (--w2 < 0)
				w2 = r.length - 1;
		}
	}
}