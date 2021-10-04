#region

using System;
using Appalachia.Audio.Components;
using Appalachia.Base.Scriptables;
using Appalachia.Editing.Attributes;
using Sirenix.OdinInspector;

#endregion

namespace Appalachia.Audio.Contextual
{
    [Serializable]
    public sealed class
        ContextualAudioPatch : SelfSavingAndIdentifyingScriptableObject<ContextualAudioPatch>
    {
        [SmartLabel]
        [HorizontalGroup("A", .8f)]
        [PropertyOrder(10)]
        public Patch patch;

        [ToggleLeft]
        [SmartLabel]
        [HorizontalGroup("A", .2f)]
        [PropertyOrder(20)]
        public bool dualPatch;

        [SmartLabel]
        [HorizontalGroup("B")]
        [PropertyOrder(30)]
        [ShowIf(nameof(dualPatch))]
        public Patch patch2;

        public static Patch operator %(ContextualAudioPatch p, bool first)
        {
            /*var p1 = p.patch != null;
            var p2 = p.patch2 != null;

            if (p1 && !p2) return p.patch;
            if (p2 && !p1) return p.patch2;*/

            return p.dualPatch
                ? first
                    ? p.patch
                    : p.patch2
                : p.patch;
        }
    }
}