using Application.Common.Options;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Persistence;

/// <summary>
/// Seed các tài khoản gốc theo cấu hình AuthOptions.SeedAccounts.
/// Idempotent: chỉ tạo user nếu CCCD chưa tồn tại (dựa vào Repository — DIP).
/// </summary>
public class AuthSeeder : IAuthSeeder
{
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _uow;
    private readonly AuthOptions _options;
    private readonly ILogger<AuthSeeder> _logger;

    public AuthSeeder(
        IUserRepository users,
        IUnitOfWork uow,
        IOptions<AuthOptions> options,
        ILogger<AuthSeeder> logger)
    {
        _users = users;
        _uow = uow;
        _options = options.Value;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {
        if (!_options.EnableSeeding)
        {
            _logger.LogInformation("[AuthSeeder] EnableSeeding=false — bỏ qua.");
            return;
        }

        if (_options.SeedAccounts.Count == 0)
        {
            _logger.LogWarning("[AuthSeeder] EnableSeeding=true nhưng SeedAccounts rỗng.");
            return;
        }

        int created = 0, skipped = 0;
        foreach (var acc in _options.SeedAccounts)
        {
            if (await _users.IsCCCDExistsAsync(acc.CCCD, ct))
            {
                skipped++;
                continue;
            }

            if (!Enum.IsDefined(typeof(UserRoleEnum), acc.Role))
            {
                _logger.LogWarning("[AuthSeeder] Role {Role} không hợp lệ, bỏ qua CCCD {Cccd}", acc.Role, acc.CCCD);
                continue;
            }

            var role = (UserRoleEnum)acc.Role;
            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = acc.FullName,
                /// Birthday mặc định 2000-01-01 cho seed, đủ hợp lệ để login.
                Birthday = new DateOnly(2000, 1, 1),
                CCCD = acc.CCCD,
                /// BCrypt hash — password lưu ở appsettings dạng plaintext, hash trước khi xuống DB.
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(acc.Password),
                Role = role,
                IsActive = true,
                IsDeleted = false,
                Discriminator = role == UserRoleEnum.Student ? "Student" : "Lecturer",
                StudentCode = role == UserRoleEnum.Student ? acc.StudentCode : null,
                Major = role == UserRoleEnum.Student ? acc.Major : null,
                LecturerCode = role == UserRoleEnum.Lecturer ? acc.LecturerCode : null,
                Subject = role == UserRoleEnum.Lecturer ? acc.Subject : null
            };

            await _users.AddAsync(user, ct);
            created++;
        }

        if (created > 0)
        {
            await _uow.SaveChangesAsync(ct);
            _logger.LogInformation("[AuthSeeder] Tạo {Created} tài khoản seed, bỏ qua {Skipped} đã tồn tại.", created, skipped);
        }
        else
        {
            _logger.LogInformation("[AuthSeeder] Không tạo tài khoản mới ({Skipped} đã tồn tại).", skipped);
        }
    }
}
