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
public abstract class AudioContextCollection3<TEnumPrimary, TEnumSecondary, TEnumTertiary, TContext,
                                                  TIndex, TDoubleIndex, TTripleIndex, 
                                                  TListPrimary, TListSecondary, TListTertiary, 
                                                  TListContext, 
                                                  TListIndex, TListDoubleIndex, 
                                                  T> : AudioContextCollection<TContext, T>
        where TEnumPrimary : Enum
        where TEnumSecondary : Enum
        where TEnumTertiary : Enum
        where TContext : AudioContext3<TEnumPrimary, TEnumSecondary, TEnumTertiary, TContext>
        where TLookup : AppaLookup<TEnumTertiary, TContext, TListTertiary, TListContext>, new()
        where TDoubleIndex : DoubleAppaList<TEnumSecondary, TEnumTertiary, TContext, TListSecondary, TListTertiary, TListContext, TIndex,
            TListIndex>, new()
        where TTripleIndex : TripleAppaList<TEnumPrimary, TEnumSecondary, TEnumTertiary, TContext, TListPrimary, TListSecondary, TListTertiary,
            TListContext, TIndex, TDoubleIndex, TListIndex, TListDoubleIndex>, new()
        where TListPrimary : AppaList<TEnumPrimary>, new()
        where TListSecondary : AppaList<TEnumSecondary>, new()
        where TListTertiary : AppaList<TEnumTertiary>, new()
        where TListContext : AppaList<TContext>, new()
        where TListIndex : AppaList<TIndex>, new()
        where TListDoubleIndex : AppaList<TDoubleIndex>, new()
        where T : AudioContextCollection3<TEnumPrimary, TEnumSecondary, TEnumTertiary, TContext, TIndex, TDoubleIndex, TTripleIndex, TListPrimary,
            TListSecondary, TListTertiary, TListContext, TListIndex, TListDoubleIndex, T>
    {
        [SerializeField, SmartLabel, InlineProperty]
        private TTripleIndex index;

        protected override void AddOrUpdate(TContext context)
        {
            index.AddOrUpdate(context.primaryContext, context.secondaryContext, context.tertiaryContext, context);
        }

        public AudioContextPatch GetBest(TEnumPrimary primary, TEnumSecondary secondary, TEnumTertiary tertiary, out bool successful)
        {
            if (index == null)
            {
                index = new TTripleIndex();
            }

            TContext fallback;

            if (index.TryGet(primary, out var subIndex))
            {
                if (subIndex == null)
                {
                    subIndex = new TDoubleIndex();
                    index[primary] = subIndex;
                }

                if (subIndex.TryGet(secondary, out var subSubIndex))
                {
                    if (subSubIndex == null)
                    {
                        subSubIndex = new TIndex();
                        subIndex[secondary] = subSubIndex;
                    }

                    if (subSubIndex.TryGet(tertiary, out var context))
                    {
                        successful = true;
                        return context.patch;
                    }

                    Debug.LogWarning($"No patch found for [{primary}, {secondary}, {tertiary}].");

                    fallback = subSubIndex.FirstWithPreference_NoAlloc(
                        si => Equals(si.primaryContext, primary) && Equals(si.secondaryContext, secondary) && si.defaultFallback,
                        out successful
                    );

                    if (successful)
                    {
                        return fallback.patch;
                    }

                    Debug.LogWarning($"No fallback patch found for [{primary}, {secondary}, {tertiary}].");
                }
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


