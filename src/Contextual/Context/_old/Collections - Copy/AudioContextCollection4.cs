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
public abstract class AudioContextCollection4<TEnumPrimary, TEnumSecondary, TEnumTertiary, TEnumQuaternary, TContext, TIndex, TDoubleIndex,
                                                  TTripleIndex, TQuadrupleIndex, TListPrimary, TListSecondary, TListTertiary, TListQuaternary,
                                                  TListContext, TListIndex, TListDoubleIndex, TListTripleIndex,
                                                  T> : AudioContextCollection<TContext, T>
        where TEnumPrimary : Enum
        where TEnumSecondary : Enum
        where TEnumTertiary : Enum
        where TEnumQuaternary : Enum
        where TContext : AudioContext4<TEnumPrimary, TEnumSecondary, TEnumTertiary, TEnumQuaternary, TContext>
        where TLookup : AppaLookup<TEnumQuaternary, TContext, TListQuaternary, TListContext>, new()
        where TDoubleIndex : DoubleAppaList<TEnumTertiary, TEnumQuaternary, TContext, TListTertiary, TListQuaternary, TListContext, TIndex,
            TListIndex>, new()
        where TTripleIndex : TripleAppaList<TEnumSecondary, TEnumTertiary, TEnumQuaternary, TContext, TListSecondary, TListTertiary,
            TListQuaternary, TListContext, TIndex, TDoubleIndex, TListIndex, TListDoubleIndex>, new()
        where TQuadrupleIndex : QuadrupleAppaList<TEnumPrimary, TEnumSecondary, TEnumTertiary, TEnumQuaternary, TContext, TListPrimary,
            TListSecondary, TListTertiary, TListQuaternary, TListContext, TIndex, TDoubleIndex, TTripleIndex, TListIndex, TListDoubleIndex,
            TListTripleIndex>, new()
        where TListPrimary : AppaList<TEnumPrimary>, new()
        where TListSecondary : AppaList<TEnumSecondary>, new()
        where TListTertiary : AppaList<TEnumTertiary>, new()
        where TListQuaternary : AppaList<TEnumQuaternary>, new()
        where TListContext : AppaList<TContext>, new()
        where TListIndex : AppaList<TIndex>, new()
        where TListDoubleIndex : AppaList<TDoubleIndex>, new()
        where TListTripleIndex : AppaList<TTripleIndex>, new()
        where T : AudioContextCollection4<TEnumPrimary, TEnumSecondary, TEnumTertiary, TEnumQuaternary, TContext, TIndex, TDoubleIndex, TTripleIndex,
            TQuadrupleIndex, TListPrimary, TListSecondary, TListTertiary, TListQuaternary, TListContext, TListIndex, TListDoubleIndex,
            TListTripleIndex, T>
    {
        [SerializeField, SmartLabel, InlineProperty]
        private TQuadrupleIndex index;

        protected override void AddOrUpdate(TContext context)
        {
            index.AddOrUpdate(context.primaryContext, context.secondaryContext, context.tertiaryContext, context.quaternaryContext, context);
        }

        public AudioContextPatch GetBest(TEnumPrimary primary, TEnumSecondary secondary, TEnumTertiary tertiary, TEnumQuaternary quaternary, out bool successful)
        {
            if (index == null)
            {
                index = new TQuadrupleIndex();
            }

            TContext fallback;

            if (index.TryGet(primary, out var sub1Index))
            {
                if (sub1Index == null)
                {
                    sub1Index = new TTripleIndex();
                    index[primary] = sub1Index;
                }

                if (sub1Index.TryGet(secondary, out var sub2Index))
                {
                    if (sub2Index == null)
                    {
                        sub2Index = new TDoubleIndex();
                        sub1Index[secondary] = sub2Index;
                    }

                    if (sub2Index.TryGet(tertiary, out var sub3Index))
                    {
                        if (sub3Index == null)
                        {
                            sub3Index = new TIndex();
                            sub2Index[tertiary] = sub3Index;
                        }

                        if (sub3Index.TryGet(quaternary, out var context))
                        {
                            successful = true;
                            return context.patch;
                        }

                        Debug.LogWarning($"No patch found for [{primary}, {secondary}, {tertiary}, {quaternary}].");

                        fallback = sub3Index.FirstWithPreference_NoAlloc(
                            si => Equals(si.primaryContext,   primary) &&
                                  Equals(si.secondaryContext, secondary) &&
                                  Equals(si.tertiaryContext,  tertiary) &&
                                  si.defaultFallback,
                            out successful
                        );

                        if (successful)
                        {
                            return fallback.patch;
                        }

                        Debug.LogWarning($"No fallback patch found for [{primary}, {secondary}, {tertiary}, {quaternary}].");
                    }
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


