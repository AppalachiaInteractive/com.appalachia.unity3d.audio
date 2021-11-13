using System;
using Appalachia.Core.Attributes.Editing;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Audio.Contextual.Context.Contexts
{
    [Serializable]
    public sealed class AudioContextParameters3 : AudioContextParameters
    {
        #region Fields

        [SerializeField]
        [SmartLabel]
        [InlineProperty]
        public AudioContextType primary;

        [SerializeField]
        [SmartLabel]
        [InlineProperty]
        public AudioContextType secondary;

        [SerializeField]
        [SmartLabel]
        [InlineProperty]
        public AudioContextType tertiary;

        #endregion
    }
}
