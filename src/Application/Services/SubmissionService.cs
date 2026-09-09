using Application.Common;
using Application.DTOs.Submissions;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class SubmissionService : ISubmissionService
{
    private readonly ISubmissionRepository _submissions;
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _uow;

    public SubmissionService(ISubmissionRepository submissions, IUserRepository users, IUnitOfWork uow)
    {
        _submissions = submissions;
        _users = users;
        _uow = uow;
    }

    public async Task<PagedResult<SubmissionDto>> GetPagedAsync(PagedRequest req, CancellationToken ct = default)
    {
        var all = await _submissions.GetAllAsync(ct);
        var total = all.Count;
        var items = all
            .OrderByDescending(s => s.UpdatedDate)
            .Skip((req.Page - 1) * req.PageSize)
            .Take(req.PageSize)
            .Select(s => ToDto(s, null))
            .ToList();
        return new PagedResult<SubmissionDto> { Items = items, TotalCount = total, Page = req.Page, PageSize = req.PageSize };
    }

    public async Task<Result<SubmissionDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var s = await _submissions.GetByIdAsync(id, ct);
        if (s == null) return Result<SubmissionDto>.Failure($"Submission '{id}' không tồn tại.", "SUBMISSION_NOT_FOUND");
        var lecturer = await _users.GetByIdAsync(s.LecturerId, ct);
        return Result<SubmissionDto>.Success(ToDto(s, lecturer?.FullName));
    }

    public async Task<Result<SubmissionDto>> CreateAsync(CreateSubmissionRequest req, Guid currentUserId, CancellationToken ct = default)
    {
        var s = new Submission
        {
            SubmissionId = Guid.NewGuid(),
            SubmissionName = req.SubmissionName,
            Folder = req.Folder,
            Status = req.Status,
            LecturerId = currentUserId,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow
        };
        await _submissions.AddAsync(s, ct);
        await _uow.SaveChangesAsync(ct);

        var lecturer = await _users.GetByIdAsync(currentUserId, ct);
        return Result<SubmissionDto>.Success(ToDto(s, lecturer?.FullName));
    }

    public async Task<Result<SubmissionDto>> UpdateAsync(Guid id, UpdateSubmissionRequest req, CancellationToken ct = default)
    {
        var s = await _submissions.GetByIdAsync(id, ct);
        if (s == null) return Result<SubmissionDto>.Failure($"Submission '{id}' không tồn tại.", "SUBMISSION_NOT_FOUND");

        if (req.SubmissionName != null) s.SubmissionName = req.SubmissionName;
        if (req.Folder != null) s.Folder = req.Folder;
        if (req.Status.HasValue) s.Status = req.Status.Value;
        s.UpdatedDate = DateTime.UtcNow;

        _submissions.Update(s);
        await _uow.SaveChangesAsync(ct);

        var lecturer = await _users.GetByIdAsync(s.LecturerId, ct);
        return Result<SubmissionDto>.Success(ToDto(s, lecturer?.FullName));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var s = await _submissions.GetByIdAsync(id, ct);
        if (s == null) return Result.Failure($"Submission '{id}' không tồn tại.", "SUBMISSION_NOT_FOUND");

        if (await _submissions.HasGradingsAsync(id, ct))
            return Result.Failure("Không thể xóa submission đã có điểm.", "SUBMISSION_HAS_GRADINGS");

        _submissions.Remove(s);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    private static SubmissionDto ToDto(Submission s, string? lecturerName) => new()
    {
        SubmissionId = s.SubmissionId,
        SubmissionName = s.SubmissionName,
        Folder = s.Folder,
        Status = s.Status,
        CreatedDate = s.CreatedDate,
        UpdatedDate = s.UpdatedDate,
        LecturerId = s.LecturerId,
        LecturerName = lecturerName
    };
}
