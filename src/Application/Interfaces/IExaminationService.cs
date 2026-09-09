using Application.Common;
using Application.DTOs.Examinations;

namespace Application.Interfaces;

/// <summary>
/// Service quản lý đợt thi / kỳ thi trong học kỳ.
/// </summary>
public interface IExaminationService
{
    Task<PagedResult<ExaminationDto>> GetPagedAsync(PagedRequest request, CancellationToken ct = default);
    Task<Result<ExaminationDto>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<ExaminationDto>> CreateAsync(CreateExaminationRequest request, CancellationToken ct = default);
    Task<Result<ExaminationDto>> UpdateAsync(Guid id, UpdateExaminationRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
}
