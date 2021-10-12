using System;
using Appalachia.Audio.Components;
using Appalachia.Audio.Contextual.Context.Collections;
using Appalachia.Audio.Contextual.Context.Contexts;
using Appalachia.Core.Behaviours;
using UnityEngine;

namespace Appalachia.Audio.Contextual.Execution
{
    [Serializable]
    public abstract class AudioExecutionProcessor<TCollection, TContext, TParams, TOwner>
        where TCollection : AudioContextCollection<TContext, TParams, TCollection>
        where TContext : AudioContext<TParams>
        where TParams : AudioContextParameters, new()
        where TOwner : AppalachiaMonoBehaviour
    {
        [SerializeField] public TCollection audio;

        [NonSerialized] private bool _initialized;
        public bool initialized => _initialized;

        public void Initialize(TOwner owner)
        {
            if (_initialized)
            {
                return;
            }

            _initialized = true;

            OnInitialize(owner);
        }

        protected abstract void OnInitialize(TOwner owner);

        public abstract bool Update(
            TOwner owner,
            out Patch patch,
            out AudioParameters.EnvelopeParams envelope,
            out Vector3 position,
            out float volume);

        public abstract void Direct(
            TOwner owner,
            out Patch patch,
            out AudioParameters.EnvelopeParams envelope,
            out Vector3 position,
            out float volume);
    }
}
