#region

using System;
using Appalachia.Audio.Contextual.Context.Contexts;
using Appalachia.Core.Attributes.Editing;
using Appalachia.Core.Objects.Initialization;
using Appalachia.Core.Objects.Root;
using Appalachia.Utility.Async;
using Sirenix.OdinInspector;
using Unity.Profiling;

#endregion

namespace Appalachia.Audio.Contextual.Context.Collections
{
    [Serializable]
    public abstract class AudioContextCollection<TContext, TParams, T> : SingletonAppalachiaObject<T>
        where TParams : AudioContextParameters
        where TContext : AudioContext<TParams>
        where T : AudioContextCollection<TContext, TParams, T>
    {
        #region Fields and Autoproperties

        [SmartLabel]
        [ToggleLeft]
        [HorizontalGroup("A", .2f)]
        public bool locked;

        [SmartLabel]
        [DisableIf(nameof(locked))]
        [HorizontalGroup("A", .8f)]
        public ContextualAudioPatch defaultPatch;

        [SmartLabel]
        [DisableIf(nameof(locked))]
        [ListDrawerSettings]
        public TContext[] contexts;

        #endregion

        protected abstract void AddOrUpdate(TContext context);

        /// <inheritdoc />
        protected override async AppaTask Initialize(Initializer initializer)
        {
            await base.Initialize(initializer);

#if UNITY_EDITOR
            using (_PRF_Initialize.Auto())
            {
                Refresh();
            }
#endif
        }

#if UNITY_EDITOR

        private static readonly ProfilerMarker _PRF_Refresh = new ProfilerMarker(_PRF_PFX + nameof(Refresh));

        [Button]
        [DisableIf(nameof(locked))]
        public void Refresh()
        {
            using (_PRF_Refresh.Auto())
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
        }
#endif
    }
}
