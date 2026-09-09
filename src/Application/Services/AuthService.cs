using Application.Common;
using Application.DTOs.Auth;
using Application.Interfaces;
using Domain.Entities;
using AI_Assisted_SWR_Grading_System.Domain.Enums;

namespace Application.Services;

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

	public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
	{
		if (await _users.IsCCCDExistsAsync(request.CCCD, ct))
			return Result<AuthResponse>.Failure("CCCD đã được đăng ký.", "CCCD_EXISTS");

		if (!Enum.IsDefined(typeof(UserRole), request.Role))
			return Result<AuthResponse>.Failure("Role không hợp lệ.", "INVALID_ROLE");

		var user = new User
		{
			FullName = request.FullName.Trim(),
			Birthday = request.Birthday,
			Cccd = request.CCCD.Trim(),
			Role = (UserRole)request.Role,
			PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
			IsActive = true,
			IsDeleted = false
		};

		await _users.AddAsync(user, ct);
		await _uow.SaveChangesAsync(ct);
		return BuildAuthResponse(user);
	}

	public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default)
	{
		var user = await _users.GetByCCCDAsync(request.CCCD.Trim(), ct);
		if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
			return Result<AuthResponse>.Failure("CCCD hoặc mật khẩu không đúng.", "INVALID_CREDENTIALS");

		if (!user.IsActive || user.IsDeleted)
			return Result<AuthResponse>.Failure("Tài khoản đã bị khóa.", "ACCOUNT_LOCKED");

		return BuildAuthResponse(user);
	}

	private Result<AuthResponse> BuildAuthResponse(User user)
	{
		var role = user.Role.ToString();
		var token = _jwt.GenerateToken(user.Id, user.FullName, (int)user.Role, role);
		return Result<AuthResponse>.Success(new AuthResponse
		{
			UserId = user.Id,
			FullName = user.FullName,
			Role = role,
			Discriminator = role,
			Token = token,
			ExpiresAt = DateTime.UtcNow.AddMinutes(60)
		});
	}
}
