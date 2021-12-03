#region

using System;
using Appalachia.Audio.Scriptables;
using Appalachia.Core.Overrides;

#endregion

namespace Appalachia.Audio.Contextual
{
    [Serializable]
    public class OverridablePatch : Overridable<Patch, OverridablePatch>
    {
        public OverridablePatch(bool overrideEnabled, Patch value) : base(overrideEnabled, value)
        {
        }

        public OverridablePatch(Overridable<Patch, OverridablePatch> value) : base(value)
        {
        }

        public OverridablePatch() : base(false, default)
        {
        }
    }
}
