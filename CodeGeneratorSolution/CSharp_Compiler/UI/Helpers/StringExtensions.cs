
using System;
using static CodeGeneratorSolution.Core.ProjectManifest;

namespace CodeGeneratorSolution.UI.Helpers
{ // أو أي مسار تستخدمه في مشروعك
    public static class StringExtensions
    {
        /// <summary>
        /// يقوم بتنظيف النص العربي للبحث: إزالة المسافات، التشكيل، الشدة، توحيد الهمزات، التاء المربوطة، والياء.
        /// خوارزمية Single-Pass لأقصى سرعة أداء.
        /// </summary>
        public static string NormalizeArabicForSearch(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            char[] buffer = new char[input.Length];
            int currentIndex = 0;

            foreach (char c in input)
            {
                // 1. تجاهل المسافات
                if (c == ' ') continue;

                // 2. السحر الجديد: تجاهل الشدة وكل حركات التشكيل فوراً!
                // هذا النطاق يضم (الفتحة، الضمة، الكسرة، التنوين، الشدة، والسكون)
                if (c >= '\u064B' && c <= '\u0652') continue;

                // 3. توحيد الحروف
                switch (c)
                {
                    case 'أ':
                    case 'إ':
                    case 'آ':
                        buffer[currentIndex++] = 'ا';
                        break;

                    case 'ة':
                        buffer[currentIndex++] = 'ه';
                        break;

                    case 'ي':
                    case 'ى':
                        buffer[currentIndex++] = 'ى';
                        break;

                    case 'ـ': // إزالة التطويل (الكشيدة)
                        continue;

                    default:
                        buffer[currentIndex++] = c;
                        break;
                }
            }

            return new string(buffer, 0, currentIndex);
        }
    }
}