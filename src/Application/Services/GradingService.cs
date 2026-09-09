using Application.Common;
using Application.DTOs.Gradings;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class GradingService : IGradingService
{
    private readonly IGradingRepository _gradings;
    private readonly ISubmissionRepository _submissions;
    private readonly IUnitOfWork _uow;

    public GradingService(IGradingRepository gradings, ISubmissionRepository submissions, IUnitOfWork uow)
    {
        _gradings = gradings;
        _submissions = submissions;
        _uow = uow;
    }

    public async Task<PagedResult<GradingDto>> GetPagedAsync(PagedRequest req, CancellationToken ct = default)
    {
        var all = await _gradings.GetAllAsync(ct);
        var total = all.Count;
        var items = all
            .OrderByDescending(g => g.UpdateDate)
            .Skip((req.Page - 1) * req.PageSize)
            .Take(req.PageSize)
            .Select(g => ToDto(g, null))
            .ToList();
        return new PagedResult<GradingDto> { Items = items, TotalCount = total, Page = req.Page, PageSize = req.PageSize };
    }

    public async Task<Result<GradingDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var g = await _gradings.GetByIdAsync(id, ct);
        if (g == null) return Result<GradingDto>.Failure($"Grading '{id}' không tồn tại.", "GRADING_NOT_FOUND");
        var sub = await _submissions.GetByIdAsync(g.SubmissionId, ct);
        return Result<GradingDto>.Success(ToDto(g, sub?.SubmissionName));
    }

    public async Task<Result<GradingDto>> CreateAsync(CreateGradingRequest req, CancellationToken ct = default)
    {
        if (await _submissions.GetByIdAsync(req.SubmissionId, ct) == null)
            return Result<GradingDto>.Failure("Submission không tồn tại.", "SUBMISSION_NOT_FOUND");

        if (await _gradings.IsCodeExistsAsync(req.GradingCode, ct))
            return Result<GradingDto>.Failure("Mã grading đã tồn tại.", "GRADING_CODE_EXISTS");

        var g = new Grading
        {
            GradingId = Guid.NewGuid(),
            GradingCode = req.GradingCode,
            AiScore = req.AiScore,
            LecturerScore = null,
            AiLogs = null,
            CreateDate = DateTime.UtcNow,
            UpdateDate = DateTime.UtcNow,
            Comment = req.Comment,
            FinalScore = null,
            Status = req.Status,
            SubmissionId = req.SubmissionId
        };
        await _gradings.AddAsync(g, ct);
        await _uow.SaveChangesAsync(ct);

        var sub = await _submissions.GetByIdAsync(g.SubmissionId, ct);
        return Result<GradingDto>.Success(ToDto(g, sub?.SubmissionName));
    }

    public async Task<Result<GradingDto>> UpdateAsync(Guid id, UpdateGradingRequest req, CancellationToken ct = default)
    {
        var g = await _gradings.GetByIdAsync(id, ct);
        if (g == null) return Result<GradingDto>.Failure($"Grading '{id}' không tồn tại.", "GRADING_NOT_FOUND");

        if (req.AiScore.HasValue) g.AiScore = req.AiScore.Value;
        if (req.LecturerScore.HasValue) g.LecturerScore = req.LecturerScore.Value;
        if (req.Comment != null) g.Comment = req.Comment;
        if (req.FinalScore.HasValue) g.FinalScore = req.FinalScore.Value;
        if (req.Status.HasValue) g.Status = req.Status.Value;
        g.UpdateDate = DateTime.UtcNow;

        _gradings.Update(g);
        await _uow.SaveChangesAsync(ct);

        var sub = await _submissions.GetByIdAsync(g.SubmissionId, ct);
        return Result<GradingDto>.Success(ToDto(g, sub?.SubmissionName));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var g = await _gradings.GetByIdAsync(id, ct);
        if (g == null) return Result.Failure($"Grading '{id}' không tồn tại.", "GRADING_NOT_FOUND");

        _gradings.Remove(g);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    private static GradingDto ToDto(Grading g, string? submissionName) => new()
    {
        GradingId = g.GradingId,
        GradingCode = g.GradingCode,
        AiScore = g.AiScore,
        LecturerScore = g.LecturerScore,
        AiLogs = g.AiLogs,
        CreateDate = g.CreateDate,
        UpdateDate = g.UpdateDate,
        Comment = g.Comment,
        FinalScore = g.FinalScore,
        Status = g.Status,
        SubmissionId = g.SubmissionId,
        SubmissionName = submissionName
    };
}
