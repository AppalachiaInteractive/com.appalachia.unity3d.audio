using System;
using Appalachia.Editing.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Audio.Contextual.Context.Contexts
{
    [Serializable]
    public sealed class AudioContextParameters1 : AudioContextParameters
    {
        [SerializeField]
        [SmartLabel]
        [InlineProperty]
        public AudioContextType primary;
    }
}
