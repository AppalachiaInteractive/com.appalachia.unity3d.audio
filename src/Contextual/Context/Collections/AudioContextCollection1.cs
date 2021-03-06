#region

using System;
using Appalachia.Audio.Contextual.Context.Contexts;
using Appalachia.Core.Collections.NonSerialized;
using Appalachia.Utility.Strings;

#endregion

namespace Appalachia.Audio.Contextual.Context.Collections
{
    [Serializable]
    public abstract class
        AudioContextCollection1<TEnumPrimary, T> : AudioContextCollection<AudioContext1,
            AudioContextParameters1, T>
        where TEnumPrimary : Enum
        where T : AudioContextCollection1<TEnumPrimary, T>
    {
        #region Fields and Autoproperties

        [NonSerialized] private NonSerializedAppaLookup<TEnumPrimary, AudioContext1> index;

        #endregion

        public ContextualAudioPatch GetBest(TEnumPrimary primary, out bool successful)
        {
            if (index == null)
            {
                index = new NonSerializedAppaLookup<TEnumPrimary, AudioContext1>();
            }

            successful = index.TryGetValueWithFallback(
                primary,
                out var context,
                si => si.defaultFallback,
                ZString.Format("No context patch found for [{0}].",          primary),
                ZString.Format("No fallback context patch found for [{0}].", primary)
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

        /// <inheritdoc />
        protected override void AddOrUpdate(AudioContext1 context)
        {
            context.parameters.primary.type = typeof(TEnumPrimary);

            index.AddOrUpdate((TEnumPrimary)(object)context.parameters.primary.value, context);
        }
    }
}
