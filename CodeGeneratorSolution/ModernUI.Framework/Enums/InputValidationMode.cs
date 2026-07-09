using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernUI.Framework.Enums
{
    // 1. تعريف قواعد الإدخال
    public enum InputValidationMode
    {
        General,     // يسمح بأي شيء، لا يغير الكيبورد
        ArabicOnly,  // يحول الكيبورد لعربي، ويمنع الإنجليزي
        EnglishOnly, // يحول الكيبورد لإنجليزي، ويمنع العربي
        NumericOnly  // يسمح بالأرقام فقط
    }
}
