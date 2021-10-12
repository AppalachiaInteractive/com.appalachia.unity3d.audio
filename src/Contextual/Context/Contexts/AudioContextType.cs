using System;
using Appalachia.Core.Attributes.Editing;
using Appalachia.Core.Types;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Audio.Contextual.Context.Contexts
{
    [Serializable]
    public sealed class AudioContextType
    {
        [SerializeField]
        [HideInInspector]
        public SerializableType type;

        [SmartLabel]
        [HorizontalGroup("A")]
        [ValueDropdown(nameof(value))]
        public short value;

        private ValueDropdownList<short> values => type.EnumValues;
    }
}
