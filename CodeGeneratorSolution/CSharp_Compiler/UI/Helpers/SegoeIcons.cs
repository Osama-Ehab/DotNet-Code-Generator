namespace CodeGeneratorSolution.CSharp_Compiler.UI.Helpers
{
    public static class SegoeIcons
    {
        // استخدام فئات متداخلة (Nested Classes) لتنظيم النسخ (Outline/Solid) لكل أيقونة

        // 1. العمليات الأساسية
        public static class Save { public const string Outline = "\uE930"; public const string Solid = "\uEC61"; }
        public static class Cancel { public const string Outline = "\uEA39"; public const string Solid = "\uEB90"; }
        public static class Edit { public const string Outline = "\uE70F"; public const string Solid = "\uE104"; } // قلم
        public static class Delete { public const string Outline = "\uE74D"; public const string Solid = "\uE74D"; } // سلة مهملات
        public static class Add { public const string Outline = "\uE710"; public const string Solid = "\uECC8"; } // إضافة
        public static class Search { public const string Outline = "\uE71E"; public const string Solid = "\uE71E"; }

        // 2. الكيانات (Entities)
        public static class Person { public const string Outline = "\uE77B"; public const string Solid = "\uEA8C"; } // شخص
        public static class Users { public const string Outline = "\uE716"; public const string Solid = "\uE902"; } // مجموعة أشخاص
        public static class Document { public const string Outline = "\uE9F9"; public const string Solid = "\uEA92"; } // مستند/شهادة

        // 3. البيانات والأنواع
        public static class Email { public const string Outline = "\uE715"; public const string Solid = "\uE8A8"; }
        public static class Phone { public const string Outline = "\uE717"; public const string Solid = "\uEA8F"; }
        public static class Password { public const string Outline = "\uE72E"; public const string Solid = "\uE72E"; } // قفل
        public static class Location { public const string Outline = "\uE816"; public const string Solid = "\uE81D"; } // خريطة
        public static class Money { public const string Outline = "\uE825"; public const string Solid = "\uE825"; }
        public static class Calendar { public const string Outline = "\uE787"; public const string Solid = "\uEA89"; }
        public static class Key { public const string Outline = "\uE13D"; public const string Solid = "\uE13D"; } // مفتاح أساسي

        // 4. واجهة المستخدم
        public static class Home { public const string Outline = "\uE80F"; public const string Solid = "\uEA8A"; }
        public static class Settings { public const string Outline = "\uE713"; public const string Solid = "\uE713"; }
        public static class Warning { public const string Outline = "\uE7BA"; public const string Solid = "\uE7BA"; }
        public static class Check { public const string Outline = "\uE73E"; public const string Solid = "\uE73E"; }

        // 5. أيقونة افتراضية
        public static class Default { public const string Outline = "\uE968"; public const string Solid = "\uE968"; }
    }
}