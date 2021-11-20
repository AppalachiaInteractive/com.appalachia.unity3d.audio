using System;
using System.Diagnostics;

namespace Appalachia.Audio.Utilities
{
    [Serializable]
    public struct TernaryBool
    {
        #region Fields and Autoproperties

        private int value;

        #endregion

        [DebuggerStepThrough]
        public static bool operator ==(TernaryBool a, bool b)
        {
            return a.ToBool(b);
        }

        [DebuggerStepThrough]
        public static implicit operator TernaryBool(bool b)
        {
            return new() {value = b ? 1 : -1};
        }

        [DebuggerStepThrough]
        public static bool operator !=(TernaryBool a, bool b)
        {
            return !a.ToBool(b);
        }

        [DebuggerStepThrough]
        public override bool Equals(object o)
        {
            return o is TernaryBool ? ((TernaryBool) o).value == value : false;
        }

        [DebuggerStepThrough]
        public override int GetHashCode()
        {
            return (value + 2) * 0x1357;
        }

        public bool ToBool(bool polarity)
        {
            return polarity ? value > 0 : value < 0;
        }
    }
}
