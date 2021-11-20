#region

using System;
using Appalachia.Core.Attributes.Editing;
using Sirenix.OdinInspector;

#endregion

namespace Appalachia.Audio.Contextual.Context.Contexts
{
    [Serializable]
    public abstract class AudioContext<T>
        where T : AudioContextParameters
    {
        #region Fields and Autoproperties

        [ToggleLeft]
        [SmartLabel]
        [HorizontalGroup("B")]
        [PropertyOrder(10)]
        public bool defaultFallback;

        [SmartLabel]
        [InlineProperty]
        [PropertyOrder(20)]
        public ContextualAudioPatch patch;

        [SmartLabel]
        [InlineProperty]
        [PropertyOrder(0)]
        public T parameters;

        #endregion
    }
}
