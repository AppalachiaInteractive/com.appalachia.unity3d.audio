#region

using System;
using Appalachia.Core.Editing.Attributes;
using Sirenix.OdinInspector;

#endregion

namespace Appalachia.Core.AssetMetadata.AudioMetadata.Context.Contexts
{
    [Serializable]
    public abstract class AudioContext<T> where T : AudioContextParameters
    {
        [SmartLabel, InlineProperty, PropertyOrder(0)]
        public T parameters;
        
        [ToggleLeft, SmartLabel, HorizontalGroup("B"), PropertyOrder(10)]
        public bool defaultFallback;

        [SmartLabel, InlineProperty, PropertyOrder(20)]
        public ContextualAudioPatch patch;
    }
}
