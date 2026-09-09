using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth;

/// <summary>
/// DTO đăng ký tài khoản mới.
/// </summary>
public class RegisterRequest
{
    [Required, StringLength(255, MinimumLength = 2)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public DateOnly Birthday { get; set; }

    [Required, StringLength(20, MinimumLength = 9)]
    public string CCCD { get; set; } = string.Empty;

    /// <summary>
    /// 0 = Student, 1 = Lecturer.
    /// </summary>
    [Required]
    public int Role { get; set; }

    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;

    // Student fields (optional - chỉ dùng nếu Role = 0)
    public string? StudentCode { get; set; }
    public string? Major { get; set; }

    // Lecturer fields (optional - chỉ dùng nếu Role = 1)
    public string? LecturerCode { get; set; }
    public string? Subject { get; set; }
}

/// <summary>
/// DTO đăng nhập bằng CCCD + password.
/// </summary>
public class LoginRequest
{
    [Required]
    public string CCCD { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Response trả về khi login/register thành công.
/// </summary>
public class AuthResponse
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Discriminator { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}
