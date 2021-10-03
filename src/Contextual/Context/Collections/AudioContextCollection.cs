#region

using System;
using Appalachia.Core.AssetMetadata.AudioMetadata.Context.Contexts;
using Appalachia.Core.Editing.Attributes;
using Appalachia.Core.Scriptables;
using Sirenix.OdinInspector;

#endregion

namespace Appalachia.Core.AssetMetadata.AudioMetadata.Context.Collections
{
    [Serializable]
    public abstract class AudioContextCollection<TContext, TParams, T> : SelfSavingSingletonScriptableObject<T>
        where TParams : AudioContextParameters
        where TContext : AudioContext<TParams>
        where T : AudioContextCollection<TContext, TParams, T>
    {
        [SmartLabel, ToggleLeft, HorizontalGroup("A", .2f)]
        public bool locked;

        [SmartLabel, DisableIf(nameof(locked)), HorizontalGroup("A", .8f)]
        public ContextualAudioPatch defaultPatch;

        [SmartLabel, DisableIf(nameof(locked))]
        [ListDrawerSettings]
        public TContext[] contexts;

        protected override void OnEnable()
        {
            base.OnEnable();
            
            Refresh();
        }

        protected abstract void AddOrUpdate(TContext context);

#if UNITY_EDITOR
        [Button, DisableIf(nameof(locked))]
        public void Refresh()
        {
            if (locked)
            {
                return;
            }

            for (var i = 0; i < contexts.Length; i++)
            {
                var ctx = contexts[i];

                AddOrUpdate(ctx);
            }
        }
#endif
    }
}
