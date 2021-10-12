using System;
using Appalachia.Core.Attributes.Editing;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Audio.Contextual.Context.Contexts
{
    [Serializable]
    public sealed class AudioContextParameters2 : AudioContextParameters
    {
        [SerializeField]
        [SmartLabel]
        [InlineProperty]
        public AudioContextType primary;

        [SerializeField]
        [SmartLabel]
        [InlineProperty]
        public AudioContextType secondary;
    }
}
