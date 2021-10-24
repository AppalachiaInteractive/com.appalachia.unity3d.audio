// #define SEQUENCER_PARANOIA

#region

using System.Collections.Generic;

#endregion

namespace Appalachia.Audio.Components
{
    public static class Sequencer
    {
        public static List<Cue> activeCues0 = new(64);
        public static List<Cue> activeCues1 = new(64);

        public static uint CueIn(AudioEmitter e, int i)
        {
            var c = new Cue {emitter = e, index = i, cueHandle = Synthesizer.GetNextHandle()};
            if (!c.KeyOn())
            {
                return 0;
            }

            activeCues0.Add(c);
            return c.cueHandle;
        }

        public static void CueOut(uint handle, float release = 0f, EnvelopeMode mode = EnvelopeMode.None)
        {
            var enumerator = activeCues0.GetEnumerator();
            
            for (var x = enumerator; x.MoveNext();)
            {
                var z = x.Current;
                if (z.cueHandle == handle)
                {
                    z.KeyOff(release, mode);
                }
                else
                {
                    activeCues1.Add(z);
                }
            }

            Swap(ref activeCues0, ref activeCues1);
            activeCues1.Clear();
        }

        public static void Reset()
        {
            activeCues0.Clear();
            activeCues1.Clear();
        }

        public static void Update(float dt)
        {
            var enumerator = activeCues0.GetEnumerator();
            for (var x = enumerator; x.MoveNext();)
            {
                var z = x.Current;
                var s = z.Update(dt);
                if (s == CueStatus.Repeating)
                {
                    z.Reset();
                    z.KeyOn();
                }

                if (s != CueStatus.Stopped)
                {
                    activeCues1.Add(z);
                }
            }

            Swap(ref activeCues0, ref activeCues1);
            activeCues1.Clear();
        }

        private static void Swap<T>(ref List<T> a, ref List<T> b)
        {
            var y = a;
            a = b;
            b = y;
        }
    }
}
