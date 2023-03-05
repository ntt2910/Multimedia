using System;

namespace BW.Stats
{
    public enum StatModType
    {
        Flat = 0,
        PercentAdd = 1,
        PercentMul = 2,
    }

    public static partial class StatModTypeHelper
    {
        public static string FormatString(this StatModType modType)
        {
            switch (modType)
            {
                case StatModType.Flat: return "{0:0}";
                case StatModType.PercentAdd: return "{0:p}";
                case StatModType.PercentMul: return "{0:p}";
                default: throw new ArgumentOutOfRangeException(nameof(modType), modType, null);
            }
        }
    }
}