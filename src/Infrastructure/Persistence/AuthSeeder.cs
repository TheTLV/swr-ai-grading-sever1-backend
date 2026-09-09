using AI_Assisted_SWR_Grading_System.Domain.Enums;
using Application.Common.Options;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Persistence;

public sealed class AuthSeeder : IAuthSeeder
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
            return;

        var created = 0;
        foreach (var account in _options.SeedAccounts)
        {
            if (string.IsNullOrWhiteSpace(account.CCCD) ||
                string.IsNullOrWhiteSpace(account.Password) ||
                !Enum.IsDefined(typeof(UserRole), account.Role) ||
                await _users.IsCCCDExistsAsync(account.CCCD, ct))
                continue;

            await _users.AddAsync(new User
            {
                FullName = account.FullName.Trim(),
                Birthday = new DateOnly(2000, 1, 1),
                Cccd = account.CCCD.Trim(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(account.Password),
                Role = (UserRole)account.Role,
                IsActive = true,
                IsDeleted = false
            }, ct);
            created++;
        }

        if (created > 0)
            await _uow.SaveChangesAsync(ct);

        _logger.LogInformation("Authentication seeding complete. Created {Created} accounts.", created);
    }
}
