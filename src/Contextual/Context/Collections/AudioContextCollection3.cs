#region

using System;
using Appalachia.Audio.Contextual.Context.Contexts;
using Appalachia.Core.Collections.NonSerialized;

#endregion

namespace Appalachia.Audio.Contextual.Context.Collections
{
    [Serializable]
    public abstract class AudioContextCollection3<TEnumPrimary, TEnumSecondary, TEnumTertiary, T> : AudioContextCollection<AudioContext3,AudioContextParameters3,  T>
        where TEnumPrimary : Enum
        where TEnumSecondary : Enum
        where TEnumTertiary : Enum
        where T : AudioContextCollection3<TEnumPrimary, TEnumSecondary, TEnumTertiary, T>
    {
        [NonSerialized] private NonSerializedAppaLookup3<TEnumPrimary, TEnumSecondary, TEnumTertiary, AudioContext3> index;

        protected override void AddOrUpdate(AudioContext3 context)
        {
            context.parameters.primary.type = typeof(TEnumPrimary);
            context.parameters.secondary.type = typeof(TEnumSecondary);
            context.parameters.tertiary.type = typeof(TEnumTertiary);
            
            index.AddOrUpdate(
                (TEnumPrimary) (object) context.parameters.primary.value,
                (TEnumSecondary) (object) context.parameters.secondary.value,
                (TEnumTertiary) (object) context.parameters.tertiary.value,
                context
            );
        }

        public ContextualAudioPatch GetBest(TEnumPrimary primary, TEnumSecondary secondary, TEnumTertiary tertiary, out bool successful)
        {
            if (index == null)
            {
                index = new NonSerializedAppaLookup3<TEnumPrimary, TEnumSecondary, TEnumTertiary, AudioContext3>();
            }

            successful = index.TryGetValueWithFallback(
                primary,
                secondary,
                tertiary,
                out var context,
                si => Equals((TEnumPrimary) (object) si.parameters.primary.value,     primary) &&
                      Equals((TEnumSecondary) (object) si.parameters.secondary.value, secondary) &&
                      si.defaultFallback,
                $"No context patch found for [{primary}, {secondary}, {tertiary}].",
                $"No fallback context patch found for [{primary}, {secondary}, {tertiary}]."
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
