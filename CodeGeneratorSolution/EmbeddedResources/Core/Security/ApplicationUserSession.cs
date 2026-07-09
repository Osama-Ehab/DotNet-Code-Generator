using {{TARGET_NAMESPACE}}.Core.Context;
using {{TARGET_NAMESPACE}}.Core.DTOs;
using {{TARGET_NAMESPACE}}.Core.Enums;
using {{TARGET_NAMESPACE}}.Core.Interfaces;

namespace {{TARGET_NAMESPACE}}.Core.Security
{
    public class ApplicationUserSession : IUserSession
    {
        public int UserId { get; private set; }
        public string FullName { get; private set; }
        public string Username { get; private set; }
        public string Role { get; private set; }

        // الاحتفاظ بالرقم المجمع للصلاحيات
        private long _userPermissions;

        public bool IsAuthenticated => UserId > 0;

        public void StartSession(UserSessionDto user)
        {
            if (user == null || !user.IsActive) return;

            UserId = user.Id;
            FullName = user.FullName;
            Username = user.Username;
            Role = user.Role;

            // تخزين رقم الـ Bitwise
            _userPermissions = user.Permissions;

            // توجيه الواجهة بناءً على حقل LanguagePreference (ar/en)
            string culture = (user.LanguagePreference?.Trim().ToLower() == "en") ? "en-US" : "ar-EG";

            // افتراض أن حقل IsDarkMode غير موجود بالداتابيز حالياً، سنعطيه قيمة افتراضية
            AppLayoutContext.Initialize(cultureCode: culture, isDarkMode: false);
        }

        // ==========================================
        // السحر الهندسي: الفحص بتقنية Bitwise AND
        // ==========================================
        public bool HasPermission(AppPermissions requiredPermission)
        {
            // 1. استثناء دائم لمدير النظام (حسب حقل Role)
            if (Role == "Admin" || Role == "SystemAdministrator")
                return true;

            // 2. الفحص السريع: (الصلاحيات الممنوحة AND الصلاحية المطلوبة) == الصلاحية المطلوبة
            long required = (long)requiredPermission;
            return (_userPermissions & required) == required;
        }

        public void EndSession()
        {
            UserId = 0;
            FullName = string.Empty;
            Username = string.Empty;
            Role = string.Empty;
            _userPermissions = 0;
        }
    }
}