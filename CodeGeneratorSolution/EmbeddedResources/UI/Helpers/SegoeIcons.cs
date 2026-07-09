using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace {{TARGET_NAMESPACE}}.UI.Helpers
{
    // نستخدم static class لأننا لن نحتاج لأخذ نسخة (Object) منه
    public static class SegoeIcons
    {
        // العمليات الأساسية
        public const string Save = "\uE74E";       // أيقونة الحفظ (قرص مرن)
        public const string Print = "\uE749";      // أيقونة الطباعة
        public const string Edit = "\uE70F";       // أيقونة التعديل (قلم)
        public const string Delete = "\uE74D";     // أيقونة الحذف (سلة مهملات)
        public const string Add = "\uE710";        // أيقونة الإضافة (+)
        public const string Search = "\uE71E";     // أيقونة البحث (عدسة مكبرة)

        // أيقونات خاصة بالمدرسة والطلاب
        public const string Student = "\uE77B";    // أيقونة شخص (تستخدم للطلاب/الموظفين)
        public const string Users = "\uE716";      // أيقونة مجموعة أشخاص (للفصول/أولياء الأمور)
        public const string Calendar = "\uE787";   // أيقونة التقويم (للجداول الدراسية/الغياب)
        public const string Money = "\uE825";      // أيقونة أموال (لحسابات الأقساط)
        public const string Report = "\uE9F9";     // أيقونة مستند (لشيت الكنترول/الشهادات)

        // واجهة المستخدم والتنقل
        public const string Home = "\uE80F";       // أيقونة الرئيسية (منزل)
        public const string Settings = "\uE713";   // أيقونة الإعدادات (ترس)
        public const string Warning = "\uE7BA";    // أيقونة تحذير (لإنذارات الغياب/التأخير المالي)
        public const string CheckMark = "\uE73E";  // أيقونة علامة صح (تم الدفع/ناجح)
        public const string Cancel = "\uE711";     // أيقونة علامة خطأ (راسب/إلغاء)
    }
}