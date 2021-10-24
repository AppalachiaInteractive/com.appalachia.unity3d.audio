#region

using System;
using Appalachia.Audio.Contextual.Context.Contexts;
using Appalachia.Core.Collections.NonSerialized;

#endregion

namespace Appalachia.Audio.Contextual.Context.Collections
{
    [Serializable]
    public abstract class AudioContextCollection4<TEnumPrimary, TEnumSecondary, TEnumTertiary,
                                                  TEnumQuaternary, T> : AudioContextCollection<AudioContext4,
        AudioContextParameters4, T>
        where TEnumPrimary : Enum
        where TEnumSecondary : Enum
        where TEnumTertiary : Enum
        where TEnumQuaternary : Enum
        where T : AudioContextCollection4<TEnumPrimary, TEnumSecondary, TEnumTertiary, TEnumQuaternary, T>
    {
        [NonSerialized]
        private NonSerializedAppaLookup4<TEnumPrimary, TEnumSecondary, TEnumTertiary, TEnumQuaternary,
            AudioContext4> index;

        public ContextualAudioPatch GetBest(
            TEnumPrimary primary,
            TEnumSecondary secondary,
            TEnumTertiary tertiary,
            TEnumQuaternary quaternary,
            out bool successful)
        {
            if (index == null)
            {
                index =
                    new NonSerializedAppaLookup4<TEnumPrimary, TEnumSecondary, TEnumTertiary, TEnumQuaternary,
                        AudioContext4>();
            }

            successful = index.TryGetValueWithFallback(
                primary,
                secondary,
                tertiary,
                quaternary,
                out var context,
                si => Equals((TEnumPrimary) (object) si.parameters.primary.value,     primary) &&
                      Equals((TEnumSecondary) (object) si.parameters.secondary.value, secondary) &&
                      Equals((TEnumTertiary) (object) si.parameters.tertiary.value,   tertiary) &&
                      si.defaultFallback,
                $"No context patch found for [{primary}, {secondary}, {tertiary}, {quaternary}].",
                $"No fallback context patch found for [{primary}, {secondary}, {tertiary}, {quaternary}]."
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

        protected override void AddOrUpdate(AudioContext4 context)
        {
            context.parameters.primary.type = typeof(TEnumPrimary);
            context.parameters.secondary.type = typeof(TEnumSecondary);
            context.parameters.tertiary.type = typeof(TEnumTertiary);
            context.parameters.quaternary.type = typeof(TEnumQuaternary);

            index.AddOrUpdate(
                (TEnumPrimary) (object) context.parameters.primary.value,
                (TEnumSecondary) (object) context.parameters.secondary.value,
                (TEnumTertiary) (object) context.parameters.tertiary.value,
                (TEnumQuaternary) (object) context.parameters.quaternary.value,
                context
            );
        }
    }
}
