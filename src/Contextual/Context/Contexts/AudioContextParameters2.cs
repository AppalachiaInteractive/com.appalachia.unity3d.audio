using System;
using Appalachia.Editing.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Audio.Contextual.Context.Contexts
{
    [Serializable]
    public sealed class AudioContextParameters2 : AudioContextParameters
    {
        [SerializeField, SmartLabel, InlineProperty] 
        public AudioContextType primary;

        [SerializeField, SmartLabel, InlineProperty] 
        public AudioContextType secondary;
    }
}