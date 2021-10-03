using System;
using Appalachia.Core.Editing.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Core.AssetMetadata.AudioMetadata.Context.Contexts
{
    [Serializable]
    public sealed class AudioContextParameters4 : AudioContextParameters
    {
        [SerializeField, SmartLabel, InlineProperty] 
        public AudioContextType primary;

        [SerializeField, SmartLabel, InlineProperty] 
        public AudioContextType secondary;

        [SerializeField, SmartLabel, InlineProperty] 
        public AudioContextType tertiary;

        [SerializeField, SmartLabel, InlineProperty] 
        public AudioContextType quaternary;
    }
}