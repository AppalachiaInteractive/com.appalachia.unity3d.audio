#region

using System;
using Appalachia.Audio.Components;
using Appalachia.Core.Overrides;

#endregion

namespace Appalachia.Audio.Contextual
{
    [Serializable]
    public class Patch_OVERRIDE : Overridable<Patch, Patch_OVERRIDE>
    {
        public Patch_OVERRIDE(bool overrideEnabled, Patch value) : base(overrideEnabled, value)
        {
        }

        public Patch_OVERRIDE(Overridable<Patch, Patch_OVERRIDE> value) : base(value)
        {
        }

        public Patch_OVERRIDE() : base(false, default)
        {
        }
    }
}
