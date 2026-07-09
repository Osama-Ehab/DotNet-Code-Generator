using CodeGeneratorSolution.Core.DTOs;

namespace CodeGeneratorSolution.Core.Interfaces
{
    public interface IUserSession
    {
        int UserId { get; }
        string FullName { get; }
        string Username { get; }
        string Role { get; }
        bool IsAuthenticated { get; }

        void StartSession(UserSessionDto user);

        // دالة فحص الصلاحيات باستخدام الـ Enum
        /// <summary>
        /// bool HasPermission(AppPermissions requiredPermission);
        /// </summary>

        void EndSession();
    }
}