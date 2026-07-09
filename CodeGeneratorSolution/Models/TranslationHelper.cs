using System;
using System.Collections.Generic;
using Humanizer;

namespace CodeGeneratorSolution.Models
{
    public static class TranslationHelper
    {
        // استخدام OrdinalIgnoreCase يضمن أن الحروف الكبيرة والصغيرة لن تسبب مشكلة (Student == student)
        private static readonly Dictionary<string, string> _dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            // ==========================================
            // 1. جداول النظام (الكيانات بصيغة المفرد)
            // ==========================================
            { "AcademicYear", "العام الدراسي" },
            { "Attendance", "سجل الحضور" },
            { "ClassRoom", "الفصل الدراسي" },
            { "FeeType", "نوع الرسوم" },
            { "Grade", "الصف الدراسي" },
            { "Nationality", "الجنسية" },
            { "Parent", "ولي الأمر" },
            { "PaymentDetail", "تفاصيل السداد" }, // أو تفاصيل الدفع
            { "Payment", "عملية السداد" },        // أو الدفعة / الوصل
            { "StudentFee", "رسوم الطالب" },
            { "Student", "الطالب" },
            { "User", "المستخدم" },
            
            // جداول تقنية / إعدادات
            { "UI_ColumnMetadata", "إعدادات واجهة المستخدم" },
            { "UIColumnMetadata", "إعدادات واجهة المستخدم" }, // تحسباً لإزالة الشرطة السفلية

            // ==========================================
            // 2. حقول شائعة (Common Columns) - كإضافة لبرنامجك
            // ==========================================
            { "Name", "الاسم" },
            { "FullName", "الاسم بالكامل" },
            { "Username", "اسم المستخدم" },
            { "Password", "كلمة المرور" },
            { "Email", "البريد الإلكتروني" },
            { "Phone", "رقم الهاتف" },
            { "Address", "العنوان" },
            { "Notes", "ملاحظات" },
            { "Amount", "المبلغ" },
            { "Date", "التاريخ" },
            { "Role", "الصلاحية" },
            { "IsActive", "نشط" },
            { "CreatedBy", "بواسطة" },
            { "CreatedDate", "تاريخ الإنشاء" },
            { "ModifiedBy", "مُعدل بواسطة" },
            { "ModifiedDate", "تاريخ التعديل" }
        };

        public static string TranslateEntityName(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName)) return string.Empty;

            // 1. تحويل اسم الجدول من الجمع إلى المفرد باستخدام Humanizer!
            // "AcademicYears" -> "AcademicYear" | "Nationalities" -> "Nationality"
            string singularName = tableName.Singularize();

            // 2. البحث عن الكلمة المفردة في القاموس
            if (_dictionary.TryGetValue(singularName, out string arabicWord))
                return arabicWord;

            // 3. إذا لم يجدها، يعيد الاسم المفرد مفصولاً بمسافات كبديل مؤقت
            // مثال: "TransportRoute" -> "Transport Route"
            return singularName.Humanize(LetterCasing.Title);
        }

        // دالة إضافية لترجمة أسماء الأعمدة (Columns) إذا احتجتها
        public static string TranslateColumnName(string columnName)
        {
            if (string.IsNullOrWhiteSpace(columnName)) return string.Empty;

            if (_dictionary.TryGetValue(columnName, out string arabicWord))
                return arabicWord;

            return columnName.Humanize(LetterCasing.Title);
        }
    }
}