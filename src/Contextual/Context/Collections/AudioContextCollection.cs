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

#if UNITY_EDITOR
        [Button]
        [DisableIf(nameof(locked))]
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

        protected abstract void AddOrUpdate(TContext context);

        protected override async AppaTask Initialize(Initializer initializer)
        {
            using (_PRF_Initialize.Auto())
            {
                await base.Initialize(initializer);

                Refresh();
            }
        }

        #region Profiling

        private const string _PRF_PFX = nameof(AudioContextCollection<TContext, TParams, T>) + ".";

        private static readonly ProfilerMarker _PRF_Initialize =
            new ProfilerMarker(_PRF_PFX + nameof(Initialize));

        #endregion
    }
}
