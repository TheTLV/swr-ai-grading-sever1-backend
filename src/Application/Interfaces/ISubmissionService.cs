using Application.Common;
using Application.DTOs.Submissions;

namespace Application.Interfaces;

/// <summary>
/// Service quản lý bài nộp của sinh viên.
/// </summary>
public interface ISubmissionService
{
    Task<PagedResult<SubmissionDto>> GetPagedAsync(PagedRequest request, CancellationToken ct = default);
    Task<Result<SubmissionDto>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<SubmissionDto>> CreateAsync(CreateSubmissionRequest request, Guid currentUserId, CancellationToken ct = default);
    Task<Result<SubmissionDto>> UpdateAsync(Guid id, UpdateSubmissionRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
}
