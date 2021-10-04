using System;
using Appalachia.Editing.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Audio.Contextual.Context.Contexts
{
    [Serializable]
    public sealed class AudioContextParameters4 : AudioContextParameters
    {
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

        [SerializeField]
        [SmartLabel]
        [InlineProperty]
        public AudioContextType quaternary;
    }
}
