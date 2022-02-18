using System;
using Appalachia.Utility.Reflection.Delegated;
using Appalachia.Utility.Reflection.Extensions;
using UnityEngine;

namespace Appalachia.Audio.Utilities
{
    public static class AudioUtil
    {
        #region Constants and Static Readonly

        private const string TYPE_NAME = "AudioUtil";

        private const string TYPE_NAMESPACE = "UnityEditor";

        #endregion

        #region Static Fields and Autoproperties

        private static Type TargetedType = ReflectionExtensions.GetByName(TYPE_NAMESPACE, TYPE_NAME);

        private static InternalStaticFunction<string[]> _GetAmbisonicDecoderPluginNames =
            new(TargetedType, nameof(GetAmbisonicDecoderPluginNames));

        private static InternalStaticFunction<AudioClip, int> _GetBitRate = new(
            TargetedType,
            nameof(GetBitRate)
        );

        private static InternalStaticFunction<AudioClip, int> _GetBitsPerSample = new(
            TargetedType,
            nameof(GetBitsPerSample)
        );

        private static InternalStaticFunction<AudioClip, int> _GetChannelCount = new(
            TargetedType,
            nameof(GetChannelCount)
        );

        private static InternalStaticFunction<MonoBehaviour, int> _GetCustomFilterChannelCount =
            new(TargetedType, nameof(GetCustomFilterChannelCount));

        private static InternalStaticFunction<MonoBehaviour, int, float> _GetCustomFilterMaxIn =
            new(TargetedType, nameof(GetCustomFilterMaxIn));

        private static InternalStaticFunction<MonoBehaviour, int, float> _GetCustomFilterMaxOut =
            new(TargetedType, nameof(GetCustomFilterMaxOut));

        private static InternalStaticFunction<MonoBehaviour, int> _GetCustomFilterProcessTime =
            new(TargetedType, nameof(GetCustomFilterProcessTime));

        private static InternalStaticFunction<AudioClip, double> _GetDuration = new(
            TargetedType,
            nameof(GetDuration)
        );

        private static InternalStaticFunction<float> _GetFMODCPUUsage = new(
            TargetedType,
            nameof(GetFMODCPUUsage)
        );

        private static InternalStaticFunction<int> _GetFMODMemoryAllocated = new(
            TargetedType,
            nameof(GetFMODMemoryAllocated)
        );

        private static InternalStaticFunction<AudioClip, int> _GetFrequency = new(
            TargetedType,
            nameof(GetFrequency)
        );

        private static InternalStaticFunction<AudioLowPassFilter, AnimationCurve> _GetLowpassCurve = new(
            TargetedType,
            nameof(GetLowpassCurve)
        );

        private static InternalStaticFunction<AudioClip, int> _GetMusicChannelCount = new(
            TargetedType,
            nameof(GetMusicChannelCount)
        );

        private static InternalStaticFunction<float> _GetPreviewClipPosition = new(
            TargetedType,
            nameof(GetPreviewClipPosition)
        );

        private static InternalStaticFunction<int> _GetPreviewClipSamplePosition =
            new(TargetedType, nameof(GetPreviewClipSamplePosition));

        private static InternalStaticFunction<AudioClip, int> _GetSampleCount = new(
            TargetedType,
            nameof(GetSampleCount)
        );

        private static InternalStaticFunction<AudioClip, AudioCompressionFormat> _GetSoundCompressionFormat =
            new(TargetedType, nameof(GetSoundCompressionFormat));

        private static InternalStaticFunction<AudioClip, int> _GetSoundSize = new(
            TargetedType,
            nameof(GetSoundSize)
        );

        private static InternalStaticFunction<AudioClip, AudioCompressionFormat>
            _GetTargetPlatformSoundCompressionFormat = new(
                TargetedType,
                nameof(GetTargetPlatformSoundCompressionFormat)
            );

        private static InternalStaticFunction<MonoBehaviour, bool> _HasAudioCallback = new(
            TargetedType,
            nameof(HasAudioCallback)
        );

        private static InternalStaticFunction<AudioClip, bool> _HasPreview = new(
            TargetedType,
            nameof(HasPreview)
        );

        private static InternalStaticFunction<bool> _IsPreviewClipPlaying = new(
            TargetedType,
            nameof(IsPreviewClipPlaying)
        );

        private static InternalStaticFunction<AudioClip, bool> _IsTrackerFile = new(
            TargetedType,
            nameof(IsTrackerFile)
        );

        private static InternalStaticRoutine<bool> _LoopPreviewClip = new(
            TargetedType,
            nameof(LoopPreviewClip)
        );

        private static InternalStaticRoutine _PausePreviewClip = new(TargetedType, nameof(PausePreviewClip));

        private static InternalStaticRoutine<AudioClip, int, bool> _PlayPreviewClip = new(
            TargetedType,
            nameof(PlayPreviewClip)
        );

        private static InternalStaticRoutine _ResumePreviewClip = new(
            TargetedType,
            nameof(ResumePreviewClip)
        );

        private static InternalStaticRoutine<Transform> _SetListenerTransform = new(
            TargetedType,
            nameof(SetListenerTransform)
        );

        private static InternalStaticRoutine<AudioClip, int> _SetPreviewClipSamplePosition =
            new(TargetedType, nameof(SetPreviewClipSamplePosition));

        private static InternalStaticRoutine _StopAllPreviewClips = new(
            TargetedType,
            nameof(StopAllPreviewClips)
        );

        private static InternalStaticRoutine _UpdateAudio = new(TargetedType, nameof(UpdateAudio));

        private static InternalStaticRoutine<bool> _SetProfilerShowAllGroups = new(
            TargetedType,
            nameof(SetProfilerShowAllGroups)
        );

        #endregion

        public static string[] GetAmbisonicDecoderPluginNames()
        {
            return _GetAmbisonicDecoderPluginNames.Invoke();
        }

        public static int GetBitRate(AudioClip clip)
        {
            return _GetBitRate.Invoke(clip);
        }

        public static int GetBitsPerSample(AudioClip clip)
        {
            return _GetBitsPerSample.Invoke(clip);
        }

        public static int GetChannelCount(AudioClip clip)
        {
            return _GetChannelCount.Invoke(clip);
        }

        public static int GetCustomFilterChannelCount(MonoBehaviour behaviour)
        {
            return _GetCustomFilterChannelCount.Invoke(behaviour);
        }

        public static float GetCustomFilterMaxIn(MonoBehaviour behaviour, int channel)
        {
            return _GetCustomFilterMaxIn.Invoke(behaviour, channel);
        }

        public static float GetCustomFilterMaxOut(MonoBehaviour behaviour, int channel)
        {
            return _GetCustomFilterMaxOut.Invoke(behaviour, channel);
        }

        public static int GetCustomFilterProcessTime(MonoBehaviour behaviour)
        {
            return _GetCustomFilterProcessTime.Invoke(behaviour);
        }

        public static double GetDuration(AudioClip clip)
        {
            return _GetDuration.Invoke(clip);
        }

        public static float GetFMODCPUUsage()
        {
            return _GetFMODCPUUsage.Invoke();
        }

        public static int GetFMODMemoryAllocated()
        {
            return _GetFMODMemoryAllocated.Invoke();
        }

        public static int GetFrequency(AudioClip clip)
        {
            return _GetFrequency.Invoke(clip);
        }

        public static AnimationCurve GetLowpassCurve(AudioLowPassFilter lowPassFilter)
        {
            return _GetLowpassCurve.Invoke(lowPassFilter);
        }

        public static int GetMusicChannelCount(AudioClip clip)
        {
            return _GetMusicChannelCount.Invoke(clip);
        }

        public static float GetPreviewClipPosition()
        {
            return _GetPreviewClipPosition.Invoke();
        }

        public static int GetPreviewClipSamplePosition()
        {
            return _GetPreviewClipSamplePosition.Invoke();
        }

        public static int GetSampleCount(AudioClip clip)
        {
            return _GetSampleCount.Invoke(clip);
        }

        public static AudioCompressionFormat GetSoundCompressionFormat(AudioClip clip)
        {
            return _GetSoundCompressionFormat.Invoke(clip);
        }

        public static int GetSoundSize(AudioClip clip)
        {
            return _GetSoundSize.Invoke(clip);
        }

        public static AudioCompressionFormat GetTargetPlatformSoundCompressionFormat(AudioClip clip)
        {
            return _GetTargetPlatformSoundCompressionFormat.Invoke(clip);
        }

        public static bool HasAudioCallback(MonoBehaviour behaviour)
        {
            return _HasAudioCallback.Invoke(behaviour);
        }

        public static bool HasPreview(AudioClip clip)
        {
            return _HasPreview.Invoke(clip);
        }

        public static bool IsPreviewClipPlaying()
        {
            return _IsPreviewClipPlaying.Invoke();
        }

        public static bool IsTrackerFile(AudioClip clip)
        {
            return _IsTrackerFile.Invoke(clip);
        }

        public static void LoopPreviewClip(bool on)
        {
            _LoopPreviewClip.Invoke(on);
        }

        public static void PausePreviewClip()
        {
            _PausePreviewClip.Invoke();
        }

        public static void PlayPreviewClip(AudioClip clip, int startSample = 0, bool loop = false)
        {
            _PlayPreviewClip.Invoke(clip, startSample, loop);
        }

        public static void ResumePreviewClip()
        {
            _ResumePreviewClip.Invoke();
        }

        public static void SetListenerTransform(Transform t)
        {
            _SetListenerTransform.Invoke(t);
        }

        public static void SetPreviewClipSamplePosition(AudioClip clip, int iSamplePosition)
        {
            _SetPreviewClipSamplePosition.Invoke(clip, iSamplePosition);
        }

        public static void StopAllPreviewClips()
        {
            _StopAllPreviewClips.Invoke();
        }

        public static void UpdateAudio()
        {
            _UpdateAudio.Invoke();
        }

        internal static void SetProfilerShowAllGroups(bool value)
        {
            _SetProfilerShowAllGroups.Invoke(value);
        }
#if UNITY_EDITOR

        private static InternalStaticFunction<AudioClip, UnityEditor.AudioImporter> _GetImporterFromClip =
            new(TargetedType, nameof(GetImporterFromClip));

        private static InternalStaticFunction<UnityEditor.AudioImporter, float[]> _GetMinMaxData = new(
            TargetedType,
            nameof(GetMinMaxData)
        );
#endif
#if UNITY_EDITOR
        public static UnityEditor.AudioImporter GetImporterFromClip(AudioClip clip)
        {
            return _GetImporterFromClip.Invoke(clip);
        }

        public static float[] GetMinMaxData(UnityEditor.AudioImporter importer)
        {
            return _GetMinMaxData.Invoke(importer);
        }
#endif
    }
}
