using System;
using System.Globalization;
using System.Threading;

namespace {{TARGET_NAMESPACE}}.Core.Context
{
    public static class AppLayoutContext
    {
        // ==========================================
        // 1. الخصائص الأساسية (Properties)
        // ==========================================
        public static bool IsArabicLayout { get; private set; } = true; // الافتراضي
        public static bool IsDarkMode { get; private set; } = false;

        // حدث (Event) ينطلق عندما تتغير اللغة، لتحديث الشاشات المفتوحة
        public static event EventHandler LayoutDirectionChanged;
        public static event EventHandler ThemeChanged;

    // ... (الخصائص السابقة IsArabicLayout و IsDarkMode) ...

    // الدالة الجديدة للتهيئة وقت الإقلاع
    public static void Initialize(string cultureCode, bool isDarkMode)
    {
        SetLanguage(cultureCode);

        // تعيين الثيم مباشرة دون استدعاء الحدث (لأن الشاشات لم تُبنى بعد)
        IsDarkMode = isDarkMode;
    }

    public static void SetLanguage(string cultureCode)
    {
        bool isArabic = cultureCode.StartsWith("ar", StringComparison.OrdinalIgnoreCase);
        IsArabicLayout = isArabic;

        // ⚠️ السطرين التاليين هما الأهم في نظام التشغيل ⚠️
        // يخبران محرك الـ .NET بتغيير لغة البرنامج بالكامل (أرقام، تواريخ، رسائل خطأ)
        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureCode);
        System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(cultureCode);

        LayoutDirectionChanged?.Invoke(null, EventArgs.Empty);
    }

    public static void ToggleTheme()
        {
            IsDarkMode = !IsDarkMode;
            ThemeChanged?.Invoke(null, EventArgs.Empty);
        }
    }
}