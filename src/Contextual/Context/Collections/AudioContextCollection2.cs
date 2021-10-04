#region

using System;
using Appalachia.Audio.Contextual.Context.Contexts;
using Appalachia.Core.Collections.NonSerialized;

#endregion

namespace Appalachia.Audio.Contextual.Context.Collections
{
    [Serializable]
    public abstract class
        AudioContextCollection2<TEnumPrimary, TEnumSecondary, T> : AudioContextCollection<
            AudioContext2, AudioContextParameters2, T>
        where TEnumPrimary : Enum
        where TEnumSecondary : Enum
        where T : AudioContextCollection2<TEnumPrimary, TEnumSecondary, T>
    {
        [NonSerialized]
        private NonSerializedAppaLookup2<TEnumPrimary, TEnumSecondary, AudioContext2> index;

        protected override void AddOrUpdate(AudioContext2 context)
        {
            context.parameters.primary.type = typeof(TEnumPrimary);
            context.parameters.secondary.type = typeof(TEnumSecondary);

            index.AddOrUpdate(
                (TEnumPrimary) (object) context.parameters.primary,
                (TEnumSecondary) (object) context.parameters.secondary,
                context
            );
        }

        public ContextualAudioPatch GetBest(
            TEnumPrimary primary,
            TEnumSecondary secondary,
            out bool successful)
        {
            if (index == null)
            {
                index = new NonSerializedAppaLookup2<TEnumPrimary, TEnumSecondary, AudioContext2>();
            }

            successful = index.TryGetValueWithFallback(
                primary,
                secondary,
                out var context,
                si => Equals((TEnumPrimary) (object) si.parameters.primary.value, primary) &&
                      si.defaultFallback,
                $"No context patch found for [{primary}, {secondary}].",
                $"No fallback context patch found for [{primary}, {secondary}]."
            );

            if (context == null)
            {
                if (defaultPatch == null)
                {
                    return null;
                }

                return defaultPatch;
            }

            return context.patch;
        }
    }
}
