using Appalachia.Core.AssetMetadata.AudioMetadata.Context.Collections;
using Appalachia.Core.AssetMetadata.AudioMetadata.Context.Contexts;
using Appalachia.Core.Audio;
using Appalachia.Core.Audio.Components;
using Appalachia.Core.Behaviours;
using UnityEngine;

namespace Appalachia.Core.AssetMetadata.AudioMetadata.Execution
{
    [DisallowMultipleComponent]
    public abstract class AudioExecutionManagerBehaviour<T>: InternalMonoBehaviour
        where T : AudioExecutionManagerBehaviour<T>
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