/*
#region

using System;
using Appalachia.Core.AssetMetadata.AudioMetadata.Context.Base.Contexts;
using Appalachia.Core.Audio;
using Appalachia.Core.Collections.Indexed;
using Appalachia.Core.Collections;
using Appalachia.Core.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Appalachia.Core.AssetMetadata.AudioMetadata.Context.Base.Collections
{
    [Serializable] 
public abstract class AudioContextCollection2<TEnumPrimary, TEnumSecondary, TContext, 
                                                  TIndex, TDoubleIndex,
                                                  TListPrimary, TListSecondary,
                                                  TListContext, TListIndex, T> : AudioContextCollection<TContext, T>
        where TEnumPrimary : Enum
        where TEnumSecondary : Enum
        where TContext : AudioContext2<TEnumPrimary, TEnumSecondary, TContext>
        where TLookup : AppaLookup<TEnumSecondary, TContext, TListSecondary, TListContext>, new()
        where TDoubleIndex : DoubleAppaList<TEnumPrimary, TEnumSecondary, TContext, TListPrimary, TListSecondary, TListContext, TIndex, TListIndex>
        , new()
        where TListPrimary : AppaList<TEnumPrimary>, new()
        where TListSecondary : AppaList<TEnumSecondary>, new()
        where TListContext : AppaList<TContext>, new()
        where TListIndex : AppaList<TIndex>, new()
        where T : AudioContextCollection2<TEnumPrimary, TEnumSecondary, TContext, TIndex, TDoubleIndex, TListPrimary, TListSecondary, TListContext,
            TListIndex, T>
    {
        [SerializeField, SmartLabel, InlineProperty]
        private TDoubleIndex index;

        protected override void AddOrUpdate(TContext context)
        {
            index.AddOrUpdate(context.primaryContext, context.secondaryContext, context);
        }

        public AudioContextPatch GetBest(TEnumPrimary primary, TEnumSecondary secondary, out bool successful)
        {
            if (index == null)
            {
                index = new TDoubleIndex();
            }

            TContext fallback;

            if (index.TryGet(primary, out var subIndex))
            {
                if (subIndex == null)
                {
                    subIndex = new TIndex();
                    index[primary] = subIndex;
                }

                if (subIndex.TryGet(secondary, out var context))
                {
                    successful = true;
                    return context.patch;
                }

                Debug.LogWarning($"No patch found for [{primary}, {secondary}].");

                fallback = subIndex.FirstWithPreference_NoAlloc(si => Equals(si.primaryContext, primary) && si.defaultFallback, out successful);

                if (successful)
                {
                    return fallback.patch;
                }

                Debug.LogWarning($"No fallback patch found for [{primary}, {secondary}].");
            }

            if (defaultPatch != null)
            {
                successful = true;
                return defaultPatch;
            }

            successful = false;
            return default;
        }
    }
}
*/


