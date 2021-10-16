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
public abstract class AudioContextCollection1<TEnum, TContext, TIndex, TEnumList, TContextList, T> : AudioContextCollection<TContext, T>
        where TEnum : Enum
        where TContext : AudioContext1<TEnum, TContext>
        where TLookup : AppaLookup<TEnum, TContext, TEnumList, TContextList>, new()
        where TEnumList : AppaList<TEnum>, new()
        where TContextList : AppaList<TContext>, new()
        where T : AudioContextCollection1<TEnum, TContext, TIndex, TEnumList, TContextList, T>
    {
        [SerializeField, SmartLabel, InlineProperty]
        public TIndex index;

        protected override void AddOrUpdate(TContext context)
        {
            index.AddOrUpdate(context.context, context);
        }

        public AudioContextPatch GetBest(TEnum context, out bool successful)
        {
            if (index == null)
            {
                index = new TIndex();
            }

            TContext fallback;

            if (index.TryGet(context, out var audioContext))
            {
                successful = true;
                return audioContext.patch;
            }

            Debug.LogWarning($"No patch found for [{context}].");

            fallback = index.FirstWithPreference_NoAlloc(si => Equals(si.context, context) && si.defaultFallback, out successful);

            Debug.LogWarning($"No patch found for [{context}].");

            if (successful)
            {
                return fallback.patch;
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


