using Application.Common;
using Application.DTOs.Semesters;

namespace Application.Interfaces;

/// <summary>
/// Service quản lý học kỳ.
/// </summary>
public interface ISemesterService
{
    Task<PagedResult<SemesterDto>> GetPagedAsync(PagedRequest request, CancellationToken ct = default);
    Task<Result<SemesterDto>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<SemesterDto>> CreateAsync(CreateSemesterRequest request, CancellationToken ct = default);
    Task<Result<SemesterDto>> UpdateAsync(Guid id, UpdateSemesterRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
}
