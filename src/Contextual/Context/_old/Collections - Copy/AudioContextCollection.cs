/*
#region

using Appalachia.Core.AssetMetadata.AudioMetadata.Context.Base.Contexts;
using Appalachia.Core.Audio;
using Appalachia.Core.Base.ScriptableObjects;
using Appalachia.Core.Editing.Attributes;
using Sirenix.OdinInspector;

#endregion

namespace Appalachia.Core.AssetMetadata.AudioMetadata.Context.Base.Collections
{
    [Serializable] 
public abstract class AudioContextCollection<TContext, T> : SelfSavingAndIdentifyingScriptableObject<T>
        where TContext : AudioContext
        where T : AudioContextCollection<TContext, T>
    {        
        [SmartLabel, ToggleLeft, HorizontalGroup("A", .2f)]
        public bool locked;

        [SmartLabel, DisableIf(nameof(locked)), HorizontalGroup("A", .8f)]
        public AudioContextPatch defaultPatch;

        [SmartLabel, DisableIf(nameof(locked))]
        [ListDrawerSettings]
        public TContext[] index;
        
        private void OnEnable()
        {
            Refresh();
        }

        protected abstract TContext[] GetAllAudioContexts();

        protected abstract void AddOrUpdate(TContext context);

#if UNITY_EDITOR
        [Button, DisableIf(nameof(locked))]
        public void Refresh()
        {
            if (locked)
            {
                return;
            }

            var contexts = GetAllAudioContexts();

            for (var i = 0; i < contexts.Length; i++)
            {
                var ctx = contexts[i];

                AddOrUpdate(ctx);
            }
        }
#endif
    }
}
*/


