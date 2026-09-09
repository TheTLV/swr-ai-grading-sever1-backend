using Application.Common;
using Application.DTOs.Gradings;

namespace Application.Interfaces;

/// <summary>
/// Service quản lý điểm / kết quả chấm.
/// </summary>
public interface IGradingService
{
    Task<PagedResult<GradingDto>> GetPagedAsync(PagedRequest request, CancellationToken ct = default);
    Task<Result<GradingDto>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<GradingDto>> CreateAsync(CreateGradingRequest request, CancellationToken ct = default);
    Task<Result<GradingDto>> UpdateAsync(Guid id, UpdateGradingRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
}
