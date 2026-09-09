using Application.Common;
using Application.DTOs.Auth;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

/// <summary>
/// Xử lý đăng ký / đăng nhập.
/// Phụ thuộc vào IUserRepository + IUnitOfWork (DIP) — không biết EF Core.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _uow;
    private readonly IJwtTokenService _jwt;

    public AuthService(IUserRepository users, IUnitOfWork uow, IJwtTokenService jwt)
    {
        _users = users;
        _uow = uow;
        _jwt = jwt;
    }

    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest req, CancellationToken ct = default)
    {
        // 1. Validate CCCD không trùng
        if (await _users.IsCCCDExistsAsync(req.CCCD, ct))
            return Result<AuthResponse>.Failure("CCCD đã được đăng ký.", "CCCD_EXISTS");

        // 2. Validate role
        if (!Enum.IsDefined(typeof(UserRoleEnum), req.Role))
            return Result<AuthResponse>.Failure("Role không hợp lệ.", "INVALID_ROLE");

        var role = (UserRoleEnum)req.Role;

        // 3. Validate student_code / lecturer_code không trùng
        if (role == UserRoleEnum.Student && !string.IsNullOrEmpty(req.StudentCode))
        {
            if (await _users.IsStudentCodeExistsAsync(req.StudentCode, ct))
                return Result<AuthResponse>.Failure("Mã sinh viên đã tồn tại.", "STUDENT_CODE_EXISTS");
        }

        if (role == UserRoleEnum.Lecturer && !string.IsNullOrEmpty(req.LecturerCode))
        {
            if (await _users.IsLecturerCodeExistsAsync(req.LecturerCode, ct))
                return Result<AuthResponse>.Failure("Mã giảng viên đã tồn tại.", "LECTURER_CODE_EXISTS");
        }

        // 4. Tạo user
        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = req.FullName,
            Birthday = req.Birthday,
            CCCD = req.CCCD,
            Role = role,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
            IsActive = true,
            IsDeleted = false,
            Discriminator = role == UserRoleEnum.Student ? "Student" : "Lecturer",
            StudentCode = role == UserRoleEnum.Student ? req.StudentCode : null,
            Major = role == UserRoleEnum.Student ? req.Major : null,
            LecturerCode = role == UserRoleEnum.Lecturer ? req.LecturerCode : null,
            Subject = role == UserRoleEnum.Lecturer ? req.Subject : null
        };

        await _users.AddAsync(user, ct);
        await _uow.SaveChangesAsync(ct);

        // 5. Trả về token
        return BuildAuthResponse(user);
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest req, CancellationToken ct = default)
    {
        var user = await _users.GetByCCCDAsync(req.CCCD, ct);
        if (user == null)
            return Result<AuthResponse>.Failure("CCCD hoặc mật khẩu không đúng.", "INVALID_CREDENTIALS");

        if (!user.IsActive)
            return Result<AuthResponse>.Failure("Tài khoản đã bị khóa.", "ACCOUNT_LOCKED");

        if (!BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            return Result<AuthResponse>.Failure("CCCD hoặc mật khẩu không đúng.", "INVALID_CREDENTIALS");

        return BuildAuthResponse(user);
    }

    private Result<AuthResponse> BuildAuthResponse(User user)
    {
        var token = _jwt.GenerateToken(user.Id, user.FullName, (int)user.Role, user.Discriminator);
        return Result<AuthResponse>.Success(new AuthResponse
        {
            UserId = user.Id,
            FullName = user.FullName,
            Role = user.Role.ToString(),
            Discriminator = user.Discriminator,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        });
    }
}
