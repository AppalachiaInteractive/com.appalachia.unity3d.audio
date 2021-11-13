using System;
using Appalachia.Core.Attributes.Editing;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Appalachia.Audio.Contextual.Context.Contexts
{
    [Serializable]
    public sealed class AudioContextParameters1 : AudioContextParameters
    {
        #region Fields

        [SerializeField]
        [SmartLabel]
        [InlineProperty]
        public AudioContextType primary;

        #endregion
    }
}
