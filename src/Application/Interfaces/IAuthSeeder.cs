namespace Application.Interfaces;

/// <summary>
/// Seed tài khoản gốc cho từng role khi DB trống — chỉ chạy khi EnableSeeding=true.
/// Implementation nằm trong Infrastructure (biết EF Core + BCrypt).
/// </summary>
public interface IAuthSeeder
{
    Task SeedAsync(CancellationToken ct = default);
}
