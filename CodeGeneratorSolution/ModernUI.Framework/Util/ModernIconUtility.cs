using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace ModernUI.Framework.Util
{
    public static class ModernIconUtility
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ParseIconText(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "";

            string cleanStr = value.Replace("\\u", "").Replace("0x", "").Trim();

            if (cleanStr.Length >= 4 && cleanStr.Length <= 5 && int.TryParse(cleanStr, NumberStyles.HexNumber, null, out int charCode))
            {
                return char.ConvertFromUtf32(charCode);
            }

            return value;
        }
    }
}