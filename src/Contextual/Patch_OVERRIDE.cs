#region

using System;
using Appalachia.Audio.Scriptables;
using Appalachia.Core.Objects.Models;

#endregion

namespace Appalachia.Audio.Contextual
{
    [Serializable]
    public class OverridablePatch : Overridable<Patch, OverridablePatch>
    {
        public OverridablePatch(bool overriding, Patch value) : base(overriding, value)
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
