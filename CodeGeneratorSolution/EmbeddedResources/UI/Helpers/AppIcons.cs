using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace {{TARGET_NAMESPACE}}.UI.Helpers
{
    public static class AppIcons
    {

        // داخل كلاس AppIcons
        private static readonly Lazy<Image> _defaultIcon = new Lazy<Image>(() => Properties.Resources.Icon_Default);
        public static Image DefaultIcon => _defaultIcon.Value;
        // 2. السجل المركزي (Registry) لربط النصوص بالصور بذكاء وبدون Reflection بطيء
        // نستخدم StringComparer.OrdinalIgnoreCase لضمان عدم حدوث مشاكل بسبب الحروف الكبيرة والصغيرة
        private static readonly Dictionary<string, Lazy<Image>> _iconRegistry;

        /// <summary>
        /// دالة سريعة جداً لاستدعاء الصورة الأصلية بناءً على اسمها النصي
        /// </summary>
        public static Image GetBaseImage(string iconName)
        {
            if (_iconRegistry.TryGetValue(iconName, out Lazy<Image> lazyImage))
            {
                return lazyImage.Value; // هنا فقط يتم استدعاء الويندوز لاستخراج الصورة إذا لم تُستخرج من قبل
            }
            return null;
        }
    }
}
