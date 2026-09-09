using Application.Common;
using Application.DTOs.Auth;

namespace Application.Interfaces;

/// <summary>
/// Service xử lý xác thực: đăng ký, đăng nhập, JWT.
/// </summary>
public interface IAuthService
{
    Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default);
}
