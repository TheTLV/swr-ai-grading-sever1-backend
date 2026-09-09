using Application.Common;
using Application.DTOs.ExamMaterials;

namespace Application.Interfaces;

/// <summary>
/// Service quản lý tài liệu thi (đề thi, rubric, đáp án).
/// </summary>
public interface IExamMaterialService
{
    Task<PagedResult<ExamMaterialDto>> GetPagedAsync(PagedRequest request, CancellationToken ct = default);
    Task<Result<ExamMaterialDto>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<ExamMaterialDto>> CreateAsync(CreateExamMaterialRequest request, Guid currentUserId, CancellationToken ct = default);
    Task<Result<ExamMaterialDto>> UpdateAsync(Guid id, UpdateExamMaterialRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
}
