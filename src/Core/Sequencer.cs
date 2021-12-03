// #define SEQUENCER_PARANOIA

#region

using System.Collections.Generic;
using Appalachia.Audio.Behaviours;

#endregion

namespace Appalachia.Audio.Core
{
    public static class Sequencer
    {
        #region Static Fields and Autoproperties

        public static List<Cue> activeCues = new(64);
        public static List<Cue> activeCuesSwap = new(64);

        #endregion

        public static uint CueIn(AudioEmitter emitter, int index)
        {
            var cue = new Cue { emitter = emitter, index = index, cueHandle = Synthesizer.GetNextHandle() };
            if (!cue.KeyOn())
            {
                return 0;
            }

            activeCues.Add(cue);

            return cue.cueHandle;
        }

        public static void CueOut(
            uint handleToStop,
            float release = 0f,
            EnvelopeMode mode = EnvelopeMode.None)
        {
            using var activeCueEnumerator = activeCues.GetEnumerator();

            while (activeCueEnumerator.MoveNext())
            {
                var activeCue = activeCueEnumerator.Current;
                if (activeCue.cueHandle == handleToStop)
                {
                    activeCue.KeyOff(release, mode);
                }
                else
                {
                    activeCuesSwap.Add(activeCue);
                }
            }

            Swap(ref activeCues, ref activeCuesSwap);
            activeCuesSwap.Clear();
        }

        public static void Reset()
        {
            activeCues.Clear();
            activeCuesSwap.Clear();
        }

        public static void Update(float dt)
        {
            using var enumerator = activeCues.GetEnumerator();

            while (enumerator.MoveNext())
            {
                var activeCue = enumerator.Current;
                var activeCueStatus = activeCue.Update(dt);

                if (activeCueStatus == CueStatus.Repeating)
                {
                    activeCue.Reset();
                    activeCue.KeyOn();
                }

                if (activeCueStatus != CueStatus.Stopped)
                {
                    activeCuesSwap.Add(activeCue);
                }
            }

            Swap(ref activeCues, ref activeCuesSwap);
            activeCuesSwap.Clear();
        }

        private static void Swap<T>(ref List<T> a, ref List<T> b)
        {
            (a, b) = (b, a);
        }
    }
}
