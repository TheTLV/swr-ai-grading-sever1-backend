namespace Application.Common.Options;

/// <summary>
/// Bind từ section "Auth" trong appsettings.json / env.
/// AuthOptions dùng cho: policy, JWT, seed accounts.
/// </summary>
public class AuthOptions
{
    public const string SectionName = "Auth";

    /// <summary>
    /// Danh sách tài khoản gốc (seed khi DB trống) — mỗi role 1 tài khoản để test.
    /// </summary>
    public List<SeedAccount> SeedAccounts { get; set; } = new();

    /// <summary>
    /// Có chạy seed khi app start không (set false ở Production).
    /// </summary>
    public bool EnableSeeding { get; set; } = false;
}

public class SeedAccount
{
    public string FullName { get; set; } = string.Empty;
    public string CCCD { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    /// <summary>0 = Admin, 1 = Lecturer, 2 = Student (khớp với UserRoleEnum).</summary>
    public int Role { get; set; }

    public string? StudentCode { get; set; }
    public string? Major { get; set; }
    public string? LecturerCode { get; set; }
    public string? Subject { get; set; }
}
