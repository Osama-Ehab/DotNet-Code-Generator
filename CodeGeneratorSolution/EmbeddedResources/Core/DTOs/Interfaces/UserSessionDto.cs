using {{TARGET_NAMESPACE}}.Core.DTOs.Interfaces;


namespace {{TARGET_NAMESPACE}}.Core.DTOs
{
    public class UserSessionDto : BaseIdentifiableDTO<int>
    {
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public long Permissions { get; set; } // رقم الـ Bitwise القادم من الداتابيز
        public string LanguagePreference { get; set; } // 'ar' أو 'en'
        public bool IsActive { get; set; }
    }
}