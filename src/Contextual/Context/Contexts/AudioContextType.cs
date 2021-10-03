using System;
using Appalachia.Core.Data;
using Appalachia.Core.Editing.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Core.AssetMetadata.AudioMetadata.Context.Contexts
{
    [Serializable]
    public sealed class AudioContextType
    {
        [SerializeField, HideInInspector]
        public SerializableType type;
        
        private ValueDropdownList<short> values => type.EnumValues;

        [SmartLabel, HorizontalGroup("A"), ValueDropdown(nameof(value))]
        public short value;
    }
}