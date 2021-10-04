using System;
using Appalachia.Core.Types;
using Appalachia.Editing.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Audio.Contextual.Context.Contexts
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