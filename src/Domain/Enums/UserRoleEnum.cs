namespace Domain.Enums;

/// <summary>
/// Vai trò của người dùng trong hệ thống.
/// Mapping: user.role (text) trên Supabase.
/// </summary>
public enum UserRoleEnum
{
    Student = 0,
    Lecturer = 1,
    Admin = 2
}
