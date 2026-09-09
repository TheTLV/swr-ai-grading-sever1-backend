using Domain.Entities;

namespace Application.Interfaces;

/// <summary>
/// Repository cho User — chỉ chứa query nghiệp vụ riêng của User.
/// Generic CRUD kế thừa từ IRepository&lt;User&gt;.
/// </summary>
public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByCCCDAsync(string cccd, CancellationToken ct = default);
    Task<User?> GetByStudentCodeAsync(string studentCode, CancellationToken ct = default);
    Task<User?> GetByLecturerCodeAsync(string lecturerCode, CancellationToken ct = default);
    Task<bool> IsCCCDExistsAsync(string cccd, CancellationToken ct = default);
    Task<bool> IsStudentCodeExistsAsync(string studentCode, CancellationToken ct = default);
    Task<bool> IsLecturerCodeExistsAsync(string lecturerCode, CancellationToken ct = default);
}
