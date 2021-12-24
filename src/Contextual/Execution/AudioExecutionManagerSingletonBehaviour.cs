using Appalachia.Audio.Contextual.Context.Collections;
using Appalachia.Audio.Contextual.Context.Contexts;
using Appalachia.Audio.Core;
using Appalachia.Core.Objects.Root;
using UnityEngine;

namespace Appalachia.Audio.Contextual.Execution
{
    [DisallowMultipleComponent]
    public abstract class AudioExecutionManagerSingletonBehaviour<T> : SingletonAppalachiaBehaviour<T>
        where T : AudioExecutionManagerSingletonBehaviour<T>
    {
        protected void HandleExecution<TProcessor, TCollection, TContext, TParams>(
            T owner,
            TProcessor manager)
            where TProcessor : AudioExecutionProcessor<TCollection, TContext, TParams, T>
            where TCollection : AudioContextCollection<TContext, TParams, TCollection>
            where TContext : AudioContext<TParams>
            where TParams : AudioContextParameters, new()
        {
            if (!manager.initialized)
            {
                manager.Initialize(owner);
            }

            if (manager.Update(owner, out var patch, out var envelope, out var position, out var volume))
            {
                Synthesizer.KeyOn(out _, patch, envelope, null, position, 0f, volume);
            }
        }
    }
}
